using Backend_RC.DTO;
using Backend_RC.Models;
using Backend_RC.Repositories;

namespace Backend_RC.Services;

public class TicketsService
{
    private readonly ITicketsRepository _repo;
    public TicketsService(ITicketsRepository repo) => _repo = repo;

    public Task<List<Tickets>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Tickets?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task AddAsync(TicketsDto dto) =>
        _repo.AddAsync(new Tickets
        {
            Title = dto.Title,
            Time = dto.Time,
            Image = dto.Image,
            Date = dto.Date
        });

    public async Task<bool> UpdateAsync(int id, TicketsDto dto)
    {
        var ticket = await _repo.GetByIdAsync(id);
        if (ticket == null) return false;

        ticket.Title = dto.Title;
        ticket.Time = dto.Time;
        ticket.Image = dto.Image;
        ticket.Date = dto.Date;

        await _repo.UpdateAsync(ticket);
        return true;
    }

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}
