using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Core.Models;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<SettingsDto>> GetSettings()
            => Ok(await _settingsService.GetSettingsAsync());

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSettings([FromBody] SettingsUpdateDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            var result = await _settingsService.UpdateSettingsAsync(dto);
            return result ? NoContent() : BadRequest();
        }
    }
}
