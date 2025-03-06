using Backend_RC.DTO;
using Backend_RC.Models;
using Backend_RC.Repositories;
using StackExchange.Redis;

namespace Backend_RC.Services;

//Он работает только через репозиторий, что делает код гибче, он не  знает про бд
//также подготовлен к тестированию через DI можно подставлять Mock для тестов
/// <summary>
/// Сервис для работы с физическими картами
/// </summary>
public interface IIndicatedCardService
{
    /// <summary>
    /// добавляет карту пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cardNumber"></param>
    /// <returns></returns>
    Task AddCardAsync(string userId, IndicatedCardRequestDto cardNumber);
    /// <summary>
    /// Получает карту пользвоателя по его ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IndicatedCardModel?> GetCardByUserIdAsync(string userId);
    /// <summary>
    /// Проверяет, есть ли карта у пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> UserHasCardAsync(string userId);
    /// <summary>
    /// Удаляет карту пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RemoveCardAsync(string userId);

    string GeneratePinCode();

    Task SavePinToRedis(string email, string enteredPin);

    Task<bool> VerifyPinAsync(string email, string enteredPin);


}

/// <summary>
/// реализация сервиса карт
/// </summary>
public class IndicatedCardService : IIndicatedCardService
{
    private readonly IIndicatedCardRepository _cardRepository;
    private readonly ILogger<IndicatedCardService> _logger;
    private readonly IDatabase _cache;
    private const string RedisKeyPrefix = "IndicatedCardPin:";

    public IndicatedCardService(IIndicatedCardRepository cardRepository, ILogger<IndicatedCardService> logger, IConnectionMultiplexer cache)
    {
        _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache.GetDatabase();
    }

    public string GeneratePinCode()
    {
        var random = new Random();
        return random.Next(100000, 1000000).ToString();
    }

    public async Task SavePinToRedis(string email, string pinCode)
    {
        try
        {
            var key = $"{RedisKeyPrefix}{email}";
            await _cache.HashSetAsync(key, new HashEntry[]
            {
                new HashEntry("code", pinCode),
                new HashEntry("attempts", 3)
            });
            _logger.LogInformation("Пин-код для {Email} сохранён в Redis", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении пин-кода в Redis для {Email}", email);
            throw;
        }
    }

    public async Task<bool> VerifyPinAsync(string email, string enteredPin)
    {
        var key = $"{RedisKeyPrefix}{email}";
        var storedPin = await _cache.HashGetAsync(key, "code");
        var attemptsLeft = await _cache.HashGetAsync(key, "attempts");

        if (!storedPin.HasValue || !attemptsLeft.HasValue)
            return false;

        if (storedPin == enteredPin)
        {
            await _cache.KeyDeleteAsync(key);
            return true;
        }
        else
        {
            int attempts = (int)attemptsLeft - 1;
            if (attempts <= 0)
                await _cache.KeyDeleteAsync(key);
            else
                await _cache.HashSetAsync(key, "attempts", attempts);

            return false;
        }
    }

    public async Task AddCardAsync(string userId, IndicatedCardRequestDto model)
    {
        if (string.IsNullOrWhiteSpace(model.CardNumber))
            throw new ArgumentNullException(nameof(model.CardNumber), "Номер карты не может быть пустым");

        if (await _cardRepository.UserHasCardAsync(userId))
            throw new InvalidOperationException("У пользователя уже есть карта.");

        var card = new IndicatedCardModel
        {
            Id = Guid.NewGuid(),
            CardNumber = model.CardNumber,
            isConfirmed = false,
            UserId = userId.ToString(),
            LinkedAt = DateTime.UtcNow
        };

        await _cardRepository.AddCardAsync(card);
    }

    public async Task<IndicatedCardModel?> GetCardByUserIdAsync(string userId)
    {
        return await _cardRepository.GetCardByUserIdAsync(userId);
    }

    public async Task<bool> UserHasCardAsync(string userId)
    {
        return await _cardRepository.UserHasCardAsync(userId);
    }

    public async Task RemoveCardAsync(string userId)
    {
        var card = await _cardRepository.GetCardByUserIdAsync(userId);
        if (card == null)
            throw new InvalidOperationException("Карта не найдена");

        await _cardRepository.RemoveCardAsync(card);
    }
}
