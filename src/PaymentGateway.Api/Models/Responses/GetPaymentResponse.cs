﻿namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}