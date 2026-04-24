#nullable enable
using System.Resources;
using System.Globalization;

namespace VaultMask.Cli.Resources;

public static class Messages
{
    private static readonly ResourceManager _resourceManager = 
        new ResourceManager("VaultMask.Cli.Resources.Messages", typeof(Messages).Assembly);

    public static string GetString(string key, params object[] args)
    {
        string? value = _resourceManager.GetString(key, CultureInfo.CurrentUICulture);
        if (value == null) return key;
        return args.Length > 0 ? string.Format(value, args) : value;
    }

    public static string AppTitle => GetString(nameof(AppTitle));
    public static string AppSubtitle => GetString(nameof(AppSubtitle));
    public static string ActivationErrorMissingKey => GetString(nameof(ActivationErrorMissingKey));
    public static string ActivationSuccess => GetString(nameof(ActivationSuccess));
    public static string ActivationErrorPrefix => GetString(nameof(ActivationErrorPrefix));
    public static string FreeEdition => GetString(nameof(FreeEdition));
    public static string PremiumEdition => GetString(nameof(PremiumEdition));
    public static string FreeTierWarning => GetString(nameof(FreeTierWarning));
    public static string ConnStringPrompt => GetString(nameof(ConnStringPrompt));
    public static string ErrorEmptyConn => GetString(nameof(ErrorEmptyConn));
    public static string ScanningSchema => GetString(nameof(ScanningSchema));
    public static string SelectTablesTitle => GetString(nameof(SelectTablesTitle));
    public static string MoreChoices => GetString(nameof(MoreChoices));
    public static string SelectInstructions => GetString(nameof(SelectInstructions));
    public static string NoTablesSelected => GetString(nameof(NoTablesSelected));
    public static string ErrorNoPk => GetString(nameof(ErrorNoPk));
    public static string SelectColumnsTitle => GetString(nameof(SelectColumnsTitle));
    public static string PremiumFeatureTag => GetString(nameof(PremiumFeatureTag));
    public static string ErrorPremiumModuleRequired => GetString(nameof(ErrorPremiumModuleRequired));
    public static string NoMaskableColumnsFound => GetString(nameof(NoMaskableColumnsFound));
    public static string ProposedMappingTitle => GetString(nameof(ProposedMappingTitle));
    public static string ColHeaderTable => GetString(nameof(ColHeaderTable));
    public static string ColHeaderColumn => GetString(nameof(ColHeaderColumn));
    public static string ColHeaderMaskType => GetString(nameof(ColHeaderMaskType));
    public static string SkippedNotify => GetString(nameof(SkippedNotify));
    public static string ConfirmExecution => GetString(nameof(ConfirmExecution));
    public static string ExecutionCancelled => GetString(nameof(ExecutionCancelled));
    public static string MaskingInProgress(string schema, string table) => GetString("MaskingInProgress", schema, table);
    public static string StatusSuccess => GetString(nameof(StatusSuccess));
    public static string ProcessCompleted => GetString(nameof(ProcessCompleted));
    public static string AppSlogan => GetString(nameof(AppSlogan));
    public static string ErrorConnFailed => GetString(nameof(ErrorConnFailed));
    public static string ErrorCritical => GetString(nameof(ErrorCritical));
}
