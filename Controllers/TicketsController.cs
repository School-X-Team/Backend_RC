using Backend_RC.DTO;
using Backend_RC.Repositories;
using Backend_RC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_RC.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{

    private readonly TicketsService _service;
    public TicketsController(TicketsService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var route = await _service.GetByIdAsync(id);
        return route == null ? NotFound() : Ok(route);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TicketsDto dto)
    {
        await _service.AddAsync(dto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TicketsDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}
