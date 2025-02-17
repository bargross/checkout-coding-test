namespace PaymentGateway.Api.Exceptions;

public class BankNotAvailableException : Exception
{
    public BankNotAvailableException(string message) : base(message) {}
}