using Backend_RC.DTO;
using Backend_RC.Models;
using Backend_RC.Repositories;

namespace Backend_RC.Services;

public class TaskService
{
    private readonly ITaskRepository _repo;
    public TaskService(ITaskRepository repo) => _repo = repo;

    public Task<List<TaskItem>> GetAllAsync() => _repo.GetAllAsync();
    public Task<TaskItem?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task AddAsync(TaskDto dto) =>
        _repo.AddAsync(new TaskItem
        {
            Title = dto.Title,
            Icon = dto.Icon,
            TaskPointStart = dto.TaskPointStart,
            TaskPointEnd = dto.TaskPointEnd,
            Image = dto.Image,
            Description = dto.Description,
            TaskDescription = dto.TaskDescription,
            Reward = dto.Reward,
            TaskCityType = dto.TaskCityType,
            TaskCityPlaceInfo = dto.TaskCityPlaceInfo,
            StartCoordinates = dto.StartCoordinates,
            EndCoordinates = dto.EndCoordinates
        });

    public async Task<bool> UpdateAsync(int id, TaskDto dto)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task == null) return false;

        task.Title = dto.Title;
        task.Icon = dto.Icon;
        task.TaskPointStart = dto.TaskPointStart;
        task.TaskPointEnd = dto.TaskPointEnd;
        task.Image = dto.Image;
        task.Description = dto.Description;
        task.Reward = dto.Reward;
        task.TaskCityType = dto.TaskCityType;
        task.TaskCityPlaceInfo = dto.TaskCityPlaceInfo;
        task.StartCoordinates = dto.StartCoordinates;
        task.EndCoordinates = dto.EndCoordinates;

        await _repo.UpdateAsync(task);
        return true;
    }

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}


