using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

public interface ITicketsRepository
{
    Task<List<Tickets>> GetAllAsync();
    Task<Tickets?> GetByIdAsync(int id);
    Task AddAsync(Tickets ticket);
    Task UpdateAsync(Tickets ticket);
    Task DeleteAsync(int id);
}

public class TicketsRepository : ITicketsRepository
{
    private readonly ApplicationDbContext _context;
    public TicketsRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Tickets>> GetAllAsync() =>
        await _context.Tickets.ToListAsync();

    public async Task<Tickets?> GetByIdAsync(int id) =>
        await _context.Tickets.FindAsync(id);

    public async Task AddAsync(Tickets ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tickets ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ticket = await GetByIdAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
