using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend_RC.Models;

namespace Backend_RC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Защищаем контроллер JWT
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Пользователь не найден");

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email
            });
        }

        // PUT: api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Пользователь не найден");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                    return BadRequest(setEmailResult.Errors);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }

    public class UpdateUserProfileDto
    {
        public required string FirstName
        {
            get; set;
        }
        public required string LastName
        {
            get; set;
        }
        public required string Email
        {
            get; set;
        }
    }
}
