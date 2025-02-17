using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentProcessor paymentProcessor) : ControllerBase
{

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetPaymentResponse), 200)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        try
        {
            var payment = paymentProcessor.GetPayment(id);

            return Ok(payment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(GetPaymentResponse), 200)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostPaymentResponse?>> ProcessPayment(PostPaymentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = await paymentProcessor.ProcessPaymentAsync(request, cancellationToken);

            return Ok(payment);
        }
        catch (BankNotAvailableException)
        {
            return StatusCode(502, "Bank validation service not available.");
        }
    }
}