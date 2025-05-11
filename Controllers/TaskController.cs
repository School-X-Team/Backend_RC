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
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    // GET: api/task/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> Get(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    // POST: api/task
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] TaskDto dto)
    {
        var created = await _taskService.CreateTaskAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT: api/task/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskDto dto)
    {
        var result = await _taskService.UpdateTaskAsync(id, dto);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/task/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
