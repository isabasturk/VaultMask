using Spectre.Console;
using VaultMask.Cli.Mappers;
using VaultMask.Application.Interfaces;
using VaultMask.Application.Services;
using VaultMask.Domain.Interfaces;
using VaultMask.Infrastructure.Repositories;
using VaultMask.Domain.Entities;
using VaultMask.Application.Models;
using VaultMask.Domain.Exceptions;
using VaultMask.Cli.Resources;
using System.Globalization;

//Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

// 1. Culture Setup (Auto-detection)
// CultureInfo.CurrentUICulture is automatically picked up by ResourceManager

// Initialize License Manager early
ILicenseManager licenseManager = new LicenseManager();

// Check for activation command
if (args.Length > 0 && args[0].Equals("activate", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {
        AnsiConsole.MarkupLine(Messages.ActivationErrorMissingKey);
        return;
    }

    try
    {
        licenseManager.Activate(args[1]);
        AnsiConsole.MarkupLine(Messages.ActivationSuccess);
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"{Messages.ActivationErrorPrefix}{ex.Message}");
    }
    return;
}

// 1. ASCII Art Logo
AnsiConsole.Write(
    new FigletText(Messages.AppTitle)
        .Centered()
        .Color(Color.Cyan1));

var isPremium = licenseManager.IsPremium();
var versionTag = isPremium ? Messages.PremiumEdition : Messages.FreeEdition;
AnsiConsole.MarkupLine($"{new string(' ', Math.Max(0, (Console.WindowWidth - 60) / 2))}[bold cyan]{Messages.AppSubtitle}[/] - {versionTag}");
AnsiConsole.WriteLine();

if (!isPremium)
{
    AnsiConsole.Write(new Panel(Messages.FreeTierWarning)
        .BorderColor(Color.Yellow)
        .Expand());
}

try
{
    // 2. Connection Persistence & Input
    var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    var connectionCachePath = Path.Combine(appData, "VaultMask", "last_connection.txt");
    string? lastConnection = File.Exists(connectionCachePath) ? File.ReadAllText(connectionCachePath).Trim() : null;

    var prompt = new TextPrompt<string>(Messages.ConnStringPrompt)
        .PromptStyle("cyan");

    if (!string.IsNullOrEmpty(lastConnection))
    {
        prompt.DefaultValue(lastConnection);
    }

    var connectionString = AnsiConsole.Prompt(prompt);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        AnsiConsole.MarkupLine(Messages.ErrorEmptyConn);
        return;
    }

    // Initialize Layers
    IDatabaseRepository repository = new SqlServerRepository(connectionString);
    IMaskingService maskingService = new MaskingService(repository, licenseManager);

    // 3. Discovery & Table Selection
    IEnumerable<TableInfo> tables = await AnsiConsole.Status()
        .StartAsync(Messages.ScanningSchema, async ctx =>
        {
            var result = await repository.GetTablesAsync();
            // If we reached here, connection is successful. Save it!
            File.WriteAllText(connectionCachePath, connectionString);
            return result;
        });

    var selectedTables = AnsiConsole.Prompt(
        new MultiSelectionPrompt<TableInfo>()
            .Title(Messages.SelectTablesTitle)
            .PageSize(10)
            .MoreChoicesText(Messages.MoreChoices)
            .InstructionsText(Messages.SelectInstructions)
            .AddChoices(tables)
            .UseConverter(t => $"{t.Schema}.{t.Name}"));

    if (selectedTables.Count == 0)
    {
        AnsiConsole.MarkupLine(Messages.NoTablesSelected);
        return;
    }

    // 4. Rule Suggestion & Manual Column Selection
    var tableRules = new Dictionary<TableInfo, List<MaskingRule>>();
    var pkColumns = new Dictionary<TableInfo, string>();

    foreach (var table in selectedTables)
    {
        var columns = await repository.GetColumnsAsync(table.Schema, table.Name);
        var pk = columns.FirstOrDefault(c => c.IsPrimaryKey)?.Name;
        
        if (pk == null)
        {
            AnsiConsole.MarkupLine(Messages.GetString("ErrorNoPk", $"{table.Schema}.{table.Name}"));
            continue;
        }
        pkColumns[table] = pk;

        // Interactive Column Selection for this Table
        List<ColumnInfo> selectedColsForTable;
        while (true)
        {
            var columnPrompt = new MultiSelectionPrompt<ColumnInfo>()
                .Title(Messages.GetString("SelectColumnsTitle", $"{table.Schema}.{table.Name}"))
                .InstructionsText(Messages.SelectInstructions)
                .PageSize(10)
                .UseConverter(c => 
                {
                    var suggestion = HeuristicMapper.Suggest(c.Name);
                    var premiumTag = (!isPremium && suggestion == MaskingType.TCKimlik) ? Messages.PremiumFeatureTag : "";
                    return $"{c.Name} [grey]({c.DataType})[/]{premiumTag}";
                });

            foreach (var col in columns.Where(c => !c.IsPrimaryKey))
            {
                var item = columnPrompt.AddChoice(col);
                if (HeuristicMapper.Suggest(col.Name).HasValue)
                {
                    item.Select();
                }
            }

            selectedColsForTable = AnsiConsole.Prompt(columnPrompt);

            // Manual Validation for Premium Features
            if (!isPremium)
            {
                var hasPremiumCol = selectedColsForTable.Any(c => HeuristicMapper.Suggest(c.Name) == MaskingType.TCKimlik);
                if (hasPremiumCol)
                {
                     AnsiConsole.MarkupLine(Messages.ErrorPremiumModuleRequired);
                     continue; // Re-prompt
                }
            }
            break; // Valid selection
        }

        if (selectedColsForTable.Count > 0)
        {
            var rules = selectedColsForTable
                .Select(c => new MaskingRule(c.Name, HeuristicMapper.Suggest(c.Name) ?? MaskingType.RandomText))
                .ToList();
            tableRules[table] = rules;
        }
    }

    // 5. Confirmation Screen
    if (tableRules.Count == 0)
    {
        AnsiConsole.MarkupLine(Messages.NoMaskableColumnsFound);
        return;
    }

    var summaryTable = new Table().Border(TableBorder.Rounded);
    summaryTable.AddColumn(Messages.ColHeaderTable);
    summaryTable.AddColumn(Messages.ColHeaderColumn);
    summaryTable.AddColumn(Messages.ColHeaderMaskType);

    foreach (var (table, rules) in tableRules)
    {
        foreach (var rule in rules)
        {
            summaryTable.AddRow(
                $"{table.Schema}.{table.Name}", 
                rule.ColumnName, 
                $"[green]{rule.Type}[/]");
        }
    }

    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Rule($"[yellow]{Messages.ProposedMappingTitle}[/]"));
    AnsiConsole.Write(summaryTable);
    AnsiConsole.WriteLine();

    // Report skipped columns
    var skippedColumns = selectedTables
        .SelectMany(t => 
        {
            return tableRules.ContainsKey(t) 
                ? new List<string>() 
                : new List<string> { $"{t.Schema}.{t.Name}" };
        })
        .ToList();

    if (skippedColumns.Count > 0)
    {
        AnsiConsole.MarkupLine(Messages.SkippedNotify);
        foreach (var skip in skippedColumns)
        {
            AnsiConsole.MarkupLine($"[grey] - {skip}[/]");
        }
        AnsiConsole.WriteLine();
    }

    if (!AnsiConsole.Confirm(Messages.ConfirmExecution))
    {
        AnsiConsole.MarkupLine(Messages.ExecutionCancelled);
        return;
    }

    // 6. Execution with Progress
    await AnsiConsole.Progress()
        .Columns(new ProgressColumn[] 
        {
            new TaskDescriptionColumn(),    
            new ProgressBarColumn(),        
            new PercentageColumn(),         
            new SpinnerColumn(),            
        })
        .StartAsync(async ctx =>
        {
            foreach (var (table, rules) in tableRules)
            {
                var task = ctx.AddTask(Messages.MaskingInProgress(table.Schema, table.Name));
                task.IsIndeterminate = true; 

                try
                {
                    await maskingService.MaskTableAsync(
                        table.Schema, 
                        table.Name, 
                        pkColumns[table], 
                        rules, 
                        batchSize: 1000);
                    
                    task.Value = 100;
                    task.StopTask();
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Premium"))
                {
                    task.Description = $"[red]HATA:[/] {table.Name}";
                    task.StopTask();
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[red]Hata:[/] {ex.Message}");
                }
            }
        });

    AnsiConsole.Write(new Rule($"[green]{Messages.StatusSuccess}[/]"));
    AnsiConsole.MarkupLine(Messages.ProcessCompleted);
    AnsiConsole.MarkupLine(Messages.AppSlogan);
}
catch (DatabaseConnectionException)
{
    AnsiConsole.MarkupLine(Messages.ErrorConnFailed);
    Environment.Exit(1);
}
catch (Exception)
{
    AnsiConsole.MarkupLine(Messages.ErrorCritical);
    Environment.Exit(1);
}
