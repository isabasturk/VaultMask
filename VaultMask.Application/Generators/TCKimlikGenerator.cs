namespace VaultMask.Application.Generators;

/// <summary>
/// Generates realistic Turkish Identification Numbers (T.C. Kimlik No) 
/// following the official checksum algorithm.
/// </summary>
public static class TCKimlikGenerator
{
    private static readonly Random _random = new();

    /// <summary>
    /// Generates a valid T.C. Kimlik Number.
    /// </summary>
    /// <returns>An 11-digit numeric string.</returns>
    public static string Generate()
    {
        int[] digits = new int[11];
        
        // First digit cannot be 0
        digits[0] = _random.Next(1, 10);
        
        for (int i = 1; i < 9; i++)
        {
            digits[i] = _random.Next(0, 10);
        }

        // 10th digit calculation
        int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        int evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        
        digits[9] = ((oddSum * 7) - evenSum) % 10;
        if (digits[9] < 0) digits[9] += 10;

        // 11th digit calculation
        int sumFirst10 = 0;
        for (int i = 0; i < 10; i++)
        {
            sumFirst10 += digits[i];
        }
        
        digits[10] = sumFirst10 % 10;

        return string.Join("", digits);
    }
}
