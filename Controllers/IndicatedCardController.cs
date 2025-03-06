using Backend_RC.DTO;
using Backend_RC.Models;
using Backend_RC.Repositories;
using Backend_RC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;

namespace Backend_RC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IndicatedCardController : ControllerBase
{
    private readonly IndicatedCardService _indicatedCardService;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IDatabase _redis;
    private readonly ILogger<IndicatedCardController> _logger;

    public IndicatedCardController(
        IndicatedCardService indicatedCardService,
        IEmailService emailService,
        IUserRepository userRepository,
        IConnectionMultiplexer redis,
        ILogger<IndicatedCardController> logger)
    {
        _indicatedCardService = indicatedCardService;
        _emailService = emailService;
        _userRepository = userRepository;
        _redis = redis.GetDatabase();
        _logger = logger;
    }

    /// <summary>
    /// Проверяет наличие привязанной карты
    /// </summary>
    [HttpGet("status")]
    [Authorize]
    public async Task<IActionResult> GetCardStatus()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var card = await _indicatedCardService.GetCardByUserIdAsync(userId);

        if (card == null)
            return NotFound("Карта не найдена.");

        return Ok(new { card.CardNumber, card.isConfirmed });
    }

    /// <summary>
    /// Отправляет пин-код на почту для подтверждения привязки карты
    /// </summary>
    [HttpPost("send-pin")]
    [Authorize]
    public async Task<IActionResult> SendPinCode()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound("Пользователь не найден.");

        if (user.IndicatedCard != null)
            return BadRequest("Карта уже привязана.");

        var pinCode = _indicatedCardService.GeneratePinCode();
        await _emailService.SendEmailAsync(user.Email, "Код подтверждения", $"Ваш код: {pinCode}");

        await _indicatedCardService.SavePinToRedis(user.Email, pinCode);

        return Ok("Пин-код отправлен.");
    }

    /// <summary>
    /// Подтверждает пин-код и дает доступ к привязке карты
    /// </summary>
    [HttpPost("validate-pin")]
    [Authorize]
    public async Task<IActionResult> ValidatePin([FromBody] IndicatedCardRequestDto model)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetUserByIdAsync(userId.ToString());

        if (user == null)
            return NotFound("Пользователь не найден.");

        var isValid = await _indicatedCardService.VerifyPinAsync(user.Email, model.PinCode);
        if (!isValid)
            return BadRequest("Неверный пин-код.");

        return Ok("Пин-код подтвержден.");
    }

    /// <summary>
    /// Привязывает пластиковую карту к пользователю
    /// </summary>
    [HttpPost("link-card")]
    [Authorize]
    public async Task<IActionResult> LinkCard([FromBody] IndicatedCardRequestDto model)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetUserByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Пользователь с ID {UserId} не найден.", userId);
            return NotFound("Пользователь не найден.");
        }
        var isValid = await _indicatedCardService.VerifyPinAsync(user.Email, model.PinCode);
        if (!isValid)
        {
            _logger.LogWarning("Неверный пин-код для пользователя с Email {Email}.", user.Email);
            return BadRequest("Неверный пин-код.");
        }
        _logger.LogInformation("Пин-код подтвержден для пользователя с Email {Email}.", user.Email);
        return Ok("Пин-код подтвержден.");
    }
}
