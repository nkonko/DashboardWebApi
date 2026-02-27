using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Core.Models;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<ActionResult<SettingsDto>> GetSettings()
            => Ok(await _settingsService.GetSettingsAsync());

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] SettingsUpdateDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            var result = await _settingsService.UpdateSettingsAsync(dto);
            return result ? NoContent() : BadRequest();
        }
    }
}
