using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptions()
            => Ok(await _subscriptionService.GetSubscriptionsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionDto>> GetSubscription(string id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            return subscription != null ? Ok(subscription) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionDto>> CreateSubscription([FromBody] SubscriptionCreateDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            var subscription = await _subscriptionService.CreateSubscriptionAsync(dto);
            return CreatedAtAction(nameof(GetSubscription), new { id = subscription.Id }, subscription);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscription(string id, [FromBody] SubscriptionUpdateDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            var result = await _subscriptionService.UpdateSubscriptionAsync(id, dto);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(string id)
        {
            var result = await _subscriptionService.DeleteSubscriptionAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
