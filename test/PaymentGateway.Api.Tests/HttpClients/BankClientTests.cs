using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.HttpClients;

public class BankClientTests
{
    private readonly IBankClient _bankClient;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    
    public BankClientTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        
        _httpClient.BaseAddress = new Uri("http://localhost");
        
        _bankClient = new BankClient(_httpClient);
    }
    
    [Fact]
    public async Task ValidateAsync_ServiceUnavailable_ThrowsBankNotAvailableException()
    {
        // Arrange
        MockResponse<PaymentValidationResponse>(HttpStatusCode.ServiceUnavailable, string.Empty);
        
        // Act
        Func<Task> act = async () => await _bankClient.ValidateAsync(new PaymentValidationRequest());
        
        // Assert
        await act.Should().ThrowAsync<BankNotAvailableException>();
    }
    
    [Fact]
    public async Task ValidateAsync_ServiceIsUp_ReturnsPaymentValidationResponse()
    {
        // Arrange
        MockResponse<PaymentValidationResponse>(HttpStatusCode.OK, "{\"authorized\": true, \"authorization_code\": \"1234\"}");
        
        // Act
        var response = await _bankClient.ValidateAsync(new PaymentValidationRequest());
        
        // Assert
        response.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ValidateAsync_NotSuccess_ThrowsHttpRequestException()
    {
        // Arrange
        MockResponse<PaymentValidationResponse>(HttpStatusCode.BadRequest, string.Empty);
        
        // Act
        Func<Task> act = async () => await _bankClient.ValidateAsync(new PaymentValidationRequest());
        
        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private void MockResponse<TResponse>(HttpStatusCode statusCode, string response)
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = statusCode,
                Content = new StringContent(response)
            });
    }
}