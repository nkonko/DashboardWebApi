using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "WebAPI"
            });
        }

        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecureHealth()
        {
            var userName = User?.Identity?.Name ?? "Unknown";
            var roles = User?.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? new List<string>();

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "WebAPI",
                authenticated = true,
                user = userName,
                roles = roles
            });
        }
    }
}
