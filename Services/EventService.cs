using Backend_RC.Models;
using Backend_RC.Repositories;

namespace Backend_RC.Services;


public interface IEventService
{
    Task<EventModel> CreateEventAsync(EventModel ev);
    Task<EventModel?> GetEventAsync(int id);
    Task<List<EventModel>> GetAllEventsAsync();
    Task UpdateEventAsync(EventModel ev);
    Task DeleteEventAsync(int id);
}
public class EventService: IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventModel> CreateEventAsync(EventModel ev)
    {
        // Здесь можно добавить валидацию и логику
        return await _eventRepository.AddEventAsync(ev);
    }

    public async Task<EventModel?> GetEventAsync(int id)
    {
        return await _eventRepository.GetEventByIdAsync(id);
    }

    public async Task<List<EventModel>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllEventsAsync();
    }
    public async Task UpdateEventAsync(EventModel ev)
    {
        // Валидация, проверка, что Event существует
        await _eventRepository.UpdateEventAsync(ev);
    }

    public async Task DeleteEventAsync(int id)
    {
        await _eventRepository.DeleteEventAsync(id);
    }
}
