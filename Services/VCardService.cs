using Backend_RC.Models;
using Backend_RC.Repositories;
using StackExchange.Redis;

namespace Backend_RC.Services;
public interface IVCardService
{
    /// <summary>
    /// Генерирует случайный 6-значный пин-код.
    /// </summary>
    /// <returns></returns>
    string GeneratePinCode();
    /// <summary>
    /// Сохраняет пин-код в Redis для указанного email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="pinCode"></param>
    /// <returns></returns>
    Task SavePinToRedis(string email, string pinCode);
    /// <summary>
    /// Валидирует пин-код, полученный из Redis
    /// </summary>
    /// <param name="email"></param>
    /// <param name="pinCode"></param>
    /// <returns></returns>
    Task<bool> ValidatePinFromRedis(string email, string pinCode);
    /// <summary>
    /// Создает виртуальную карту для пользователя и созраняет её в БД.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<VCardModel> CreateVCardForUser(User user);
}
public class VCardService : IVCardService
{
    private readonly IDatabase _cache;
    private readonly IVCardRepository _vCardRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VCardService> _logger;
    private readonly ICardNumberGenerator _numberGenerator;

    private const string RedisKeyPrefix = "VCardPin:";
    private readonly TimeSpan _pinExpiry = TimeSpan.FromMinutes(5);

    public VCardService(IConnectionMultiplexer redis, IVCardRepository vCardRepository, IUserRepository userRepository, ILogger<VCardService> logger, ICardNumberGenerator numberGenerator)
    {
        _cache = redis.GetDatabase();
        _vCardRepository = vCardRepository;
        _userRepository = userRepository;
        _logger = logger;
        _numberGenerator = numberGenerator;
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
                new HashEntry("code",pinCode),
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

    public async Task<bool> ValidatePinFromRedis(string email, string pinCode)
    {
        try
        {
            var key = $"{RedisKeyPrefix}{email}";
            var attempts = await _cache.HashGetAsync(key, "attempts");
            if (attempts.IsNullOrEmpty || int.Parse(attempts) <= 0)
            {
                _logger.LogWarning("Для {Email} превышено количество попыток ввода кода", email);
                return false;
            }
            var storedPin = await _cache.HashGetAsync(key, "code");
            if (string.IsNullOrEmpty(storedPin))
            {
                _logger.LogWarning("Для {Email} пин-код не найден в Redis.", email);
                return false;
            }
            return storedPin == pinCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при валидации пин-кода для {Email}", email);
            return false;
        }
    }

    public async Task<VCardModel> CreateVCardForUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException("user");

        if (user.VirtualCard != null)
        {
            _logger.LogWarning("У пользователя {UserId} уже имеется виртуальная карта.", user.Id);
            return user.VirtualCard;
        }

        var cardNumber = _numberGenerator.GenerateCardNumber();
        var vCard = new VCardModel
        {
            CardNumber = cardNumber,
            isConfirmed = true,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            User = user
        };

        await _vCardRepository.AddVCardAsync(vCard);

        user.VirtualCard = vCard;
        await _userRepository.UpdateUserAsync(user);

        _logger.LogInformation("Виртуальная карта {CardNumber} создана для пользователя {UserId}.", cardNumber, user.Id);
        return vCard;
    }
}
