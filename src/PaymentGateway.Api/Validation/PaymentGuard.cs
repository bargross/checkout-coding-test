namespace PaymentGateway.Api.Validation;

public static class PaymentGuard
{
    public static bool IsValidCardNumber(string cardNumber)
    {
        var trimmedNumber = cardNumber.Replace(" ", "");

        var hasValidCharacters = HasCharactersBetween(14, 19, trimmedNumber);
        var isNumericString = IsNumericOnly(trimmedNumber);
        
        return hasValidCharacters && isNumericString;
    }

    public static bool IsValidFutureDate(int month, int year)
    {
        if (!IsValidMonth(month))
        {
            return false;
        };
        
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        return (currentYear == year || year > currentYear) && month > currentMonth;
    }

    public static bool IsValidCurrency(string currency) => currency.Length == 3;

    public static bool IsValidCVV(string cvv) => HasCharactersBetween(3, 4, cvv) && IsNumericOnly(cvv);
    
    private static bool HasCharactersBetween(int start, int end, string characters)
    {
        var charactersLength = characters.Length;

        return charactersLength >= start && charactersLength <= end;
    }

    private static bool IsValidMonth(int month)
    {
        return month >= 1 && month <= 12;
    }
    
    private static bool IsNumericOnly(string value)
    {
        var trimmedValue = value.Replace(" ", "");
        
        var parsed = Int64.TryParse(trimmedValue, out _);

        return parsed;
    }
}