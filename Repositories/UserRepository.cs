using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserWithBonusPointsByIdAsync(string userId);
    Task UpdateUserAsync(User user);
}

/// <summary>
/// Реализация репозитория пользователей
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получение пользователя по индификатору с загрузкой сявзанный данных(карты)
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.VirtualCard)
            .Include(u => u.IndicatedCard)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Обновление данных пользователя
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserWithBonusPointsByIdAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.VirtualCard)
            .Include(u => u.IndicatedCard)
            .Include(u => u.BonusPoints)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
