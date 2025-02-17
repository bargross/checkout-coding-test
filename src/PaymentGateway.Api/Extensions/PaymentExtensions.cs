using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Extensions;

public static class PaymentExtensions
{
    public static GetPaymentResponse ToGetPaymentResponse(this Payment payment)
    {
        if (payment == null)
        {
            return null;
        }
        
        return new GetPaymentResponse
        {
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount,
            ExpiryMonth = payment.ExpiryMonth,
            CardNumberLastFour = payment.CardNumberLastFour,
            Id = payment.Id.ToString(),
            Status = payment.Status.ToString(),
        };
    }
    
    public static PostPaymentResponse ToPostPaymentResponse(this Payment payment)
    {
        if (payment == null)
        {
            return null;
        }
        
        return new PostPaymentResponse
        {
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount,
            ExpiryMonth = payment.ExpiryMonth,
            CardNumberLastFour = payment.CardNumberLastFour,
            Id = payment.Id.ToString(),
            Status = payment.Status.ToString(),
        };
    }
}