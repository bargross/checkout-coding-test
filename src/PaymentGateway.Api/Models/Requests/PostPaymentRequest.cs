﻿using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [Required] public string CardNumberLastFour { get; set; }
    [Required] public int ExpiryMonth { get; set; }
    [Required] public int ExpiryYear { get; set; }
    [Required] public string Currency { get; set; }
    [Required] public int Amount { get; set; }
    [Required] public string Cvv { get; set; }
}