using Backend_RC.DTO;
using Backend_RC.Services;
using Microsoft.AspNetCore.Mvc;


namespace Backend_RC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{

    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    // GET: api/task
    [HttpGet]
    public async Task<IActionResult> GetAll() => 
        Ok(await _taskService.GetAllAsync());

    // GET: api/task/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    // POST: api/task
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskDto dto)
    {
        await _taskService.AddAsync(dto);
        return Ok();
    }

    // PUT: api/task/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskDto dto)
    {
        var result = await _taskService.UpdateAsync(id, dto);
        return result ? Ok() : NotFound();
    }

    // DELETE: api/task/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteAsync(id);
        return Ok();
    }
}
