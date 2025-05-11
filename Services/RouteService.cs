using Backend_RC.DTO;
using Backend_RC.Models;
using Backend_RC.Repositories;

namespace Backend_RC.Services;

public class RouteService
{
    private readonly IRouteRepository _repo;
    public RouteService(IRouteRepository repo) => _repo = repo;

    public Task<List<Routee>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Routee?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task AddAsync(RouteDto dto) =>
        _repo.AddAsync(new Routee
        {
            Title = dto.Title,
            Time = dto.Time,
            Image = dto.Image,
            Description = dto.Description,
            StartCoordinates = dto.StartCoordinates,
            EndCoordinates = dto.EndCoordinates
        });

    public async Task<bool> UpdateAsync(int id, RouteDto dto)
    {
        var route = await _repo.GetByIdAsync(id);
        if (route == null) return false;

        route.Title = dto.Title;
        route.Time = dto.Time;
        route.Image = dto.Image;
        route.Description = dto.Description;
        route.StartCoordinates = dto.StartCoordinates;
        route.EndCoordinates = dto.EndCoordinates;

        await _repo.UpdateAsync(route);
        return true;
    }

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}

