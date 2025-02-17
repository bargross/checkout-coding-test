using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentControllerProcessPaymentTests
{
    private readonly Random _random = new();
    
    private readonly HttpClient _client;
    private readonly PaymentsRepository _repository;
    private readonly Mock<IBankClient> _bankClientMock;

    public PaymentControllerProcessPaymentTests()
    {
        _repository = new PaymentsRepository();
        _bankClientMock = new Mock<IBankClient>();
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        
        _client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton(_repository)
                    .AddSingleton(_bankClientMock.Object)))
            .CreateClient();
    }
    
    
    [Theory]
    [InlineData("1111 2222 3333 4444 5555")]
    [InlineData("1111 2222 3333")]
    [InlineData("A111 2b22 3333")]
    public async Task RequestsProcessingOfPayment_InvalidCardNumber_ReturnsRejectedProcessedPayment(string cardNumber)
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = cardNumber,
            Currency = "GBP",
            Cvv = "889"
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentValidationResponse { Authorized = true, AuthorizationCode = "12345" });
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        var content = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Status.Should().Be(PaymentStatus.Rejected.ToString());
    }
    
    [Theory]
    [InlineData(0, 2025)]
    [InlineData(13, 2025)]
    [InlineData(4, 2024)]
    public async Task RequestsProcessingOfPayment_InvalidExpiryDate_ReturnsRejectedProcessedPayment(int expiryMonth, int expiryYear)
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = expiryYear,
            ExpiryMonth = expiryMonth,
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = "1111 2222 3333 4444",
            Currency = "GBP",
            Cvv = "889"
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentValidationResponse { Authorized = true, AuthorizationCode = "12345" });
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        var content = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Status.Should().Be(PaymentStatus.Rejected.ToString());
    }
    
    [Theory]
    [InlineData("GBRD")]
    [InlineData("GB")]
    public async Task RequestsProcessingOfPayment_InvalidCurrency_ReturnsRejectedProcessedPayment(string currency)
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = 2025,
            ExpiryMonth = 12,
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = "1111 2222 3333 4444",
            Currency = currency,
            Cvv = "889"
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentValidationResponse { Authorized = true, AuthorizationCode = "12345" });
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        var content = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Status.Should().Be(PaymentStatus.Rejected.ToString());
    }
    
    [Theory]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("1a2B")]
    public async Task RequestsProcessingOfPayment_InvalidCvv_ReturnsRejectedProcessedPayment(string cvv)
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = 2025,
            ExpiryMonth = 12,
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = "1111 2222 3333 4444",
            Currency = "GBR",
            Cvv = cvv
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentValidationResponse { Authorized = true, AuthorizationCode = "12345" });
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        var content = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Status.Should().Be(PaymentStatus.Rejected.ToString());
    }
    
    [Fact]
    public async Task RequestsProcessingOfPayment_BankServiceDeclinesPayment_ReturnsDeclinedProcessedPayment()
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = 2025,
            ExpiryMonth = 12,
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = "1111 2222 3333 4444",
            Currency = "GBR",
            Cvv = "889"
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentValidationResponse { Authorized = false, AuthorizationCode = "12345" });
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        var content = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Status.Should().Be(PaymentStatus.Declined.ToString());
    }
    
    [Fact]
    public async Task RequestsProcessingOfPayment_BankServiceUnavailable_ReturnsBadGateway()
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = 2025,
            ExpiryMonth = 12,
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = "1111 2222 3333 4444",
            Currency = "GBR",
            Cvv = "889",
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BankNotAvailableException("bank service not available!!"));
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }
    
    [Fact]
    public async Task RequestsProcessingOfPayment_RequestIsValidAndBankServiceIsAvailable_ReturnsOKWithAuthorizedPayment()
    {
        var request = new PostPaymentRequest
        {
            ExpiryYear = 2025,
            ExpiryMonth = 12,
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = "1111 2222 3333 4444",
            Currency = "GBR",
            Cvv = "889"
        };

        _bankClientMock.Setup(x => x.ValidateAsync(It.IsAny<PaymentValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentValidationResponse { Authorized = true, AuthorizationCode = "12345" });
        
        
        var response = await _client.PostAsJsonAsync("/api/Payments", request);

        var content = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Status.Should().Be(PaymentStatus.Authorized.ToString());
    }
}