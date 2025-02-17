using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentProcessor
{
    GetPaymentResponse GetPayment(Guid paymentId);
    Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest getPaymentResponse, CancellationToken  cancellationToken = default);
}