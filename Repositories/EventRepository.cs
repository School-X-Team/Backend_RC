using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

public interface IEventRepository
{
    Task<EventModel> AddEventAsync(EventModel ev);
    Task<EventModel?> GetEventByIdAsync(int id);
    Task<List<EventModel>> GetAllEventsAsync();
    Task UpdateEventAsync(EventModel ev);
    Task DeleteEventAsync(int id);
}
public class EventRepository: IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EventModel> AddEventAsync(EventModel ev)
    {
        await _context.Events.AddAsync(ev);
        await _context.SaveChangesAsync();
        return ev;
    }

    public async Task<EventModel?> GetEventByIdAsync(int id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<List<EventModel>> GetAllEventsAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task UpdateEventAsync(EventModel ev)
    {
        _context.Events.Update(ev);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev != null)
        {
            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
        }
    }
}
