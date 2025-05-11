using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

public interface IRouteRepository
{
    Task<List<Routee>> GetAllAsync();
    Task<Routee?> GetByIdAsync(int id);
    Task AddAsync(Routee route);
    Task UpdateAsync(Routee route);
    Task DeleteAsync(int id);
}


public class RouteRepository : IRouteRepository
{
    private readonly ApplicationDbContext _context;
    public RouteRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Routee>> GetAllAsync() =>
        await _context.Routes.ToListAsync();

    public async Task<Routee?> GetByIdAsync(int id) =>
        await _context.Routes.FindAsync(id);

    public async Task AddAsync(Routee route)
    {
        await _context.Routes.AddAsync(route);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Routee route)
    {
        _context.Routes.Update(route);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var route = await GetByIdAsync(id);
        if (route != null)
        {
            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
        }
    }
}
