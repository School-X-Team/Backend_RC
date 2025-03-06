using Backend_RC.Models;

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
