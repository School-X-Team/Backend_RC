using Backend_RC.DTO;
using Backend_RC.Repositories;

namespace Backend_RC.Services;

public class TaskService
{
    private readonly TaskRepository _repository;

    public TaskService(TaskRepository repository)
    {
        _repository = repository;
    }

    public Task<List<TaskDto>> GetAllTasksAsync() => _repository.GetAllAsync();

    public Task<TaskDto?> GetTaskByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task<TaskDto> CreateTaskAsync(TaskDto dto) => _repository.CreateAsync(dto);

    public Task<bool> UpdateTaskAsync(int id, TaskDto dto) => _repository.UpdateAsync(id, dto);

    public Task<bool> DeleteTaskAsync(int id) => _repository.DeleteAsync(id);
}


