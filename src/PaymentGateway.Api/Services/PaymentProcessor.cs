using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Validation;

namespace PaymentGateway.Api.Services;

public class PaymentProcessor(PaymentsRepository repository, IBankClient client): IPaymentProcessor
{
    public GetPaymentResponse GetPayment(Guid paymentId)
    {
        var payment = repository.Get(paymentId);
        if (payment == null)
        {
            throw new KeyNotFoundException($"Payment with Id {paymentId} not found.");
        }
        
        return payment.ToGetPaymentResponse();
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request, CancellationToken  cancellationToken = default)
    {
        var response = new Payment
        {
            Id = Guid.NewGuid(),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            CardNumberLastFour = request.CardNumberLastFour.Trim([' ']),
            Amount = request.Amount,
            Status = PaymentStatus.Authorized
        };
        
        if (!PaymentGuard.IsValidCardNumber(request.CardNumberLastFour) ||
            !PaymentGuard.IsValidFutureDate(request.ExpiryMonth, request.ExpiryYear) ||
            !PaymentGuard.IsValidCurrency(request.Currency) ||
            !PaymentGuard.IsValidCVV(request.Cvv))
        {
            response.Status = PaymentStatus.Rejected;

            return response.ToPostPaymentResponse();
        }
        
        var validationResult = await client.ValidateAsync(request.ToPaymentValidationRequest(), cancellationToken);
        if (validationResult.Authorized)
        {
            response.Status = PaymentStatus.Authorized;
        }
        else
        {
            response.Status = PaymentStatus.Declined;
        }
        
        repository.Add(response);

        return response.ToPostPaymentResponse();
    }
}