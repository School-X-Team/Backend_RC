using System.Security.Claims;
using Backend_RC.DTO;
using Backend_RC.Repositories;
using Backend_RC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Backend_RC.Controllers;
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class VCardController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly VCardService _vCardService;
    private readonly UserRepository _userRepository;
    private readonly IDatabase _redisDatabase;

    public VCardController(IEmailService emailService, 
        VCardService vCardService, 
        UserRepository userRepository,
        IConnectionMultiplexer redis)
    {
        _emailService = emailService;
        _vCardService = vCardService;
        _userRepository = userRepository;
        _redisDatabase = redis.GetDatabase();
    }

    /// <summary>
    /// Запрос на выпуск вритуальной карты
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
            return NotFound();

        if (user.VirtualCard != null || user.IndicatedCard != null)
            return Conflict("К пользователю уже привязана карта.");

        string pinCode = _vCardService.GeneratePinCode();
        await _emailService.SendEmailAsync(user.Email, "Код подтверждения", $"Ваш код: {pinCode}");

        await _vCardService.SavePinToRedis(user.Email, pinCode);

        return Ok("Пин-код отправлен на email.");
    }
    /// <summary>
    /// Подтверждение пин-кода и выпуск карты
    /// </summary>
    /// <returns></returns>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmVCard([FromBody] VCardRequestDto requestDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Пользователь не авторизован.");

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null || user.Email != requestDto.Email)
            return NotFound("Пользователь не найден");

        bool isValidPin = await _vCardService.ValidatePinFromRedis(user.Email, requestDto.PinCode);

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
        return Ok();
    }
}
