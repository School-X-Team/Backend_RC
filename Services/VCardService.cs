using Backend_RC.Models;
using Backend_RC.Repositories;
using StackExchange.Redis;

namespace Backend_RC.Services;

public class VCardService : IVCardService
{
    private readonly IDatabase _cache;
    private readonly VCardRepository _vCardRepository;
    private readonly UserRepository _userRepository;
    private readonly ILogger<VCardService> _logger;
    private readonly CardNumberGenerator _numberGenerator;

    private const string RedisKeyPrefix = "VCardPin:";
    private readonly TimeSpan _pinExpiry = TimeSpan.FromMinutes(5);

    public VCardService(IConnectionMultiplexer redis, VCardRepository vCardRepository, UserRepository userRepository, ILogger<VCardService> logger, CardNumberGenerator numberGenerator)
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
