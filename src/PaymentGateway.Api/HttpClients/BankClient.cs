using System.Net;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.HttpClients;

public class BankClient(HttpClient client): IBankClient
{
    public async Task<PaymentValidationResponse> ValidateAsync(PaymentValidationRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("payments", request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            throw new BankNotAvailableException("Bank validation service is currently not available.");
        }
        
        response.EnsureSuccessStatusCode();

        return await response.Content?.ReadFromJsonAsync<PaymentValidationResponse>(cancellationToken);
    }
}