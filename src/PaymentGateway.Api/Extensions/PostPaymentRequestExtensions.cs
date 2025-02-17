using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Extensions;

public static class PostPaymentRequestExtensions
{
    public static PaymentValidationRequest ToPaymentValidationRequest(this PostPaymentRequest request)
    {
        if (request == null)
        {
            return null;
        }

        return new PaymentValidationRequest
        {
            ExpiryDate = $"{request.ExpiryMonth}/{request.ExpiryYear}",
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv,
            CardNumber = request.CardNumberLastFour.Trim([' ']),
        };
    }
}