using Backend_RC.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_RC.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BonusPointsController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public BonusPointsController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Возвращает список бонусных баллов пользователя.
    /// </summary>
    /// <returns>Список бонусов</returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetBonusPointsList()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Пользователь не авторизован.");

        var user = await _userRepository.GetUserWithBonusPointsByIdAsync(userId);
        if (user == null)
            return NotFound("Пользователь не найден.");

        return Ok(user.BonusPoints);
    }

    /// <summary>
    /// Возвращает суммарное количество бонусных баллов пользователя.
    /// </summary>
    /// <returns>Объект с суммарным количеством бонусов</returns>
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalBonusPoints()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Пользователь не авторизован.");

        var user = await _userRepository.GetUserWithBonusPointsByIdAsync(userId);
        if (user == null)
            return NotFound("Пользователь не найден.");

        int totalPoints = user.BonusPoints.Sum(bp => bp.Points);
        return Ok(new { TotalBonusPoints = totalPoints });
    }
}
