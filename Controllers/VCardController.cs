using System.Security.Claims;
using Backend_RC.DTO;
using Backend_RC.Repositories;
using Backend_RC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend_RC.Controllers;
[Route("api/VCard")]
[Authorize]
[ApiController]
public class VCardController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IVCardService _vCardService;
    private readonly IUserRepository _userRepository;
    private readonly IDatabase _redisDatabase;

    public VCardController(IEmailService emailService, 
        IVCardService vCardService, 
        IUserRepository userRepository,
        IConnectionMultiplexer redis)
    {
        _emailService = emailService;
        _vCardService = vCardService;
        _userRepository = userRepository;
        _redisDatabase = redis.GetDatabase();
    }

    /// <summary>
    /// Запрос на выпуск вритуальной карты
    /// Отправляет пин-код на email пользователя
    /// </summary>
    /// <returns>Отправка пинкода</returns>
    [HttpPost("request")]
    public async Task<IActionResult> RequestVCard()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Пользователь не авторизован.");
        
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound("Пользователь не найден");

        if (user.VirtualCard != null || user.IndicatedCard != null)
            return Conflict("К пользователю уже привязана карта.");

        string pinCode = _vCardService.GeneratePinCode();
        await _emailService.SendEmailAsync(user.Email, "Код подтверждения", $"Ваш код: {pinCode}");

        await _vCardService.SavePinToRedis(user.Email, pinCode);

        return Ok("Пин-код отправлен на email.");
    }

    /// <summary>
    /// Подтверждение пин-кода и выпуск виртуальной карты.
    /// Для подтверждения используется только пин-код, email не требуется.
    /// </summary>
    /// <param name="requestDto">Объект с пин-кодом</param>
    /// <returns>Информация о выпущенной виртуальной карте</returns>
    
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmVCard([FromBody] string pinCode)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Пользователь не авторизован.");

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound("Пользователь не найден");

        bool isValidPin = await _vCardService.ValidatePinFromRedis(user.Email, pinCode);
        if (!isValidPin)
            return BadRequest("Неверный пин-код");

        var vCard = await _vCardService.CreateVCardForUser(user);
        return Ok(new VCardResponseDto
        {
            CardNumber = vCard.CardNumber,
            IsConfirmed = vCard.isConfirmed,
            CreatedAt = vCard.CreatedAt
        });
    }

    /// <summary>
    /// Получение информации о виртуальной карте
    /// </summary>
    /// <returns></returns>
    [HttpGet("info")]
    public async Task<IActionResult> Info()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Пользователь не авторизован.");

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound("Пользователь не найден");

        return Ok(new
        {
            CardNumber = user.VirtualCard.CardNumber,
            IsConfirmed = user.VirtualCard.isConfirmed,
            CreatedAt = user.VirtualCard.CreatedAt
        });
    }
}
