namespace PaymentGateway.Api.Validation;

public static class Guard
{
    public static void ThrowIfNullEmptyOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName ?? "Value"} cannot be null, empty or whitespace.");
        }
    }
}