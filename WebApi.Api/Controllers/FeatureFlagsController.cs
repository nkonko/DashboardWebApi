using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Core.Models;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureFlagsController : ControllerBase
    {
        private readonly IFeatureFlagService _featureFlagService;

        public FeatureFlagsController(IFeatureFlagService featureFlagService)
        {
            _featureFlagService = featureFlagService;
        }

        /// <summary>
        /// Get all feature flags (Admin only - full details)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FeatureFlagDto>>> GetAll()
        {
            var flags = await _featureFlagService.GetAllAsync();
            return Ok(flags);
        }

        /// <summary>
        /// Get all feature flag states (Any authenticated user - simplified)
        /// </summary>
        [HttpGet("states")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FeatureFlagStateDto>>> GetAllStates()
        {
            var states = await _featureFlagService.GetAllStatesAsync();
            return Ok(states);
        }

        /// <summary>
        /// Check if a specific feature is enabled
        /// </summary>
        [HttpGet("check/{name}")]
        [Authorize]
        public async Task<ActionResult<bool>> IsEnabled(string name)
        {
            var isEnabled = await _featureFlagService.IsEnabledAsync(name);
            return Ok(isEnabled);
        }

        /// <summary>
        /// Get a feature flag by ID (Admin only)
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeatureFlagDto>> GetById(int id)
        {
            var flag = await _featureFlagService.GetByIdAsync(id);
            if (flag == null)
                return NotFound();
            return Ok(flag);
        }

        /// <summary>
        /// Get a feature flag by name (Admin only)
        /// </summary>
        [HttpGet("by-name/{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeatureFlagDto>> GetByName(string name)
        {
            var flag = await _featureFlagService.GetByNameAsync(name);
            if (flag == null)
                return NotFound();
            return Ok(flag);
        }

        /// <summary>
        /// Create a new feature flag (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeatureFlagDto>> Create([FromBody] FeatureFlagCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email);
                var flag = await _featureFlagService.CreateAsync(dto, userName);
                return CreatedAtAction(nameof(GetById), new { id = flag.Id }, flag);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update a feature flag (Admin only)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeatureFlagDto>> Update(int id, [FromBody] FeatureFlagUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email);
            var flag = await _featureFlagService.UpdateAsync(id, dto, userName);
            
            if (flag == null)
                return NotFound();
            
            return Ok(flag);
        }

        /// <summary>
        /// Toggle a feature flag on/off (Admin only)
        /// </summary>
        [HttpPatch("{id:int}/toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeatureFlagDto>> Toggle(int id, [FromBody] FeatureFlagToggleDto dto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email);
            var flag = await _featureFlagService.ToggleAsync(id, dto.IsEnabled, userName);
            
            if (flag == null)
                return NotFound();
            
            return Ok(flag);
        }

        /// <summary>
        /// Delete a feature flag (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _featureFlagService.DeleteAsync(id);
            if (!result)
                return NotFound();
            
            return NoContent();
        }
    }
}
