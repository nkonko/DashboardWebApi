using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] PaymentWebhookDto dto)
        {
            var result = await _paymentService.ProcessWebhookAsync(dto);
            return result ? Ok(new { message = "Webhook processed successfully" }) : BadRequest();
        }

        [HttpGet("webhooks")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PaymentWebhookResponseDto>>> GetWebhookHistory()
            => Ok(await _paymentService.GetWebhookHistoryAsync());
    }
}
