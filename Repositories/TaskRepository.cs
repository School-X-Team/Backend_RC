using Backend_RC.DTO;
using Backend_RC.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Repositories;

public class TaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskDto>> GetAllAsync()
    {
        return await _context.Tasks
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Icon = t.Icon,
                TaskPointStart = t.TaskPointStart,
                TaskPointEnd = t.TaskPointEnd,
                Image = t.Image
            }).ToListAsync();
    }

    public async Task<TaskDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Tasks.FindAsync(id);
        if (entity == null) return null;

        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Icon = entity.Icon,
            TaskPointStart = entity.TaskPointStart,
            TaskPointEnd = entity.TaskPointEnd,
            Image = entity.Image
        };
    }

    public async Task<TaskDto> CreateAsync(TaskDto dto)
    {
        var entity = new TaskItem
        {
            Title = dto.Title,
            Icon = dto.Icon,
            TaskPointStart = dto.TaskPointStart,
            TaskPointEnd = dto.TaskPointEnd,
            Image = dto.Image
        };

        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        return dto;
    }

    public async Task<bool> UpdateAsync(int id, TaskDto dto)
    {
        var entity = await _context.Tasks.FindAsync(id);
        if (entity == null) return false;

        entity.Title = dto.Title;
        entity.Icon = dto.Icon;
        entity.TaskPointStart = dto.TaskPointStart;
        entity.TaskPointEnd = dto.TaskPointEnd;
        entity.Image = dto.Image;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Tasks.FindAsync(id);
        if (entity == null) return false;

        _context.Tasks.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}


