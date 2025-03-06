using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с пластиковыми картами.
/// </summary>
public interface IIndicatedCardRepository
{
    /// <summary>
    /// Добавляет карту в БД.
    /// </summary>
    Task AddCardAsync(IndicatedCardModel card);

    /// <summary>
    /// Получает физическую карту по идентификатору пользователя.
    /// </summary>
    Task<IndicatedCardModel?> GetCardByUserIdAsync(string userId);

    /// <summary>
    /// Проверяет, существует ли у пользователя карта.
    /// </summary>
    Task<bool> UserHasCardAsync(string userId);

    /// <summary>
    /// Удаляет карту из БД.
    /// </summary>
    Task RemoveCardAsync(IndicatedCardModel card);

    Task<List<IndicatedCardModel>> GetPendingRequestsAsync();
    Task UpdateRequestAsync(IndicatedCardModel request);
}

/// <summary>
/// Репозиторий для работы с физическими картами.
/// </summary>
public class IndicatedCardRepository : IIndicatedCardRepository
{
    private readonly ApplicationDbContext _context;

    public IndicatedCardRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task AddCardAsync(IndicatedCardModel card)
    {
        if (card == null)
            throw new ArgumentNullException(nameof(card));

        await _context.IndicatedCards.AddAsync(card);
        await _context.SaveChangesAsync();
    }

    public async Task<IndicatedCardModel?> GetCardByUserIdAsync(string userId)
    {
        return await _context.IndicatedCards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<bool> UserHasCardAsync(string userId)
    {
        return await _context.IndicatedCards
            .AnyAsync(x => x.UserId == userId)
            .ConfigureAwait(false); //для избегания лишнего контекста синхронизации
    }

    public async Task RemoveCardAsync(IndicatedCardModel card)
    {
        if (card == null)
            throw new ArgumentNullException(nameof(card));

        _context.IndicatedCards.Remove(card);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IndicatedCardModel>> GetPendingRequestsAsync()
    {
        return await _context.IndicatedCards
           .Where(c => !c.isConfirmed)
           .ToListAsync();
    }

    public async Task UpdateRequestAsync(IndicatedCardModel request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _context.IndicatedCards.Update(request);
        await _context.SaveChangesAsync();
    }
}
