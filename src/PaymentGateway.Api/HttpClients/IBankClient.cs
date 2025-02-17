using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.HttpClients;

public interface IBankClient
{
    Task<PaymentValidationResponse> ValidateAsync(PaymentValidationRequest request, CancellationToken cancellationToken = default);
}