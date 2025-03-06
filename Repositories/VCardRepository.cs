using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

public interface IVCardRepository
{
    /// <summary>
    /// добавляет виртуальную карту в БД
    /// </summary>
    /// <param name="vCard"></param>
    /// <returns></returns>
    Task AddVCardAsync(VCardModel vCard);
    /// <summary>
    /// Получает виртуальную карту по индификатору пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<VCardModel?> GetVCardByUserIdAsync(string userId);
    /// <summary>
    /// Проверяет, сущетвует ли у пользователя виртуальная карта
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> UserHasVCardAsync(string userId);
    /// <summary>
    /// Удаляет виртуальную карту из БД
    /// </summary>
    /// <param name="vCard"></param>
    /// <returns></returns>
    Task RemoveVCardAsync(VCardModel vCard);
}
public class VCardRepository : IVCardRepository
{
    private readonly ApplicationDbContext _context;

    public VCardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddVCardAsync(VCardModel vCard)
    {
        if(vCard == null) 
            throw new ArgumentNullException(nameof(vCard));

        await _context.VCards.AddAsync(vCard);
        await _context.SaveChangesAsync();
    }

    public async Task<VCardModel?> GetVCardByUserIdAsync(string userId)
    {
        return await _context.VCards.FirstOrDefaultAsync(x=> x.UserId == userId);
    }

    public async Task<bool> UserHasVCardAsync(string userId)
    {
        return await _context.VCards.AnyAsync(x=> x.UserId == userId);
    }

    public async Task RemoveVCardAsync(VCardModel vCard)
    {
        if (vCard == null)
            throw new ArgumentNullException(nameof(vCard));

        _context.VCards.Remove(vCard);
        await _context.SaveChangesAsync();
    }
}
