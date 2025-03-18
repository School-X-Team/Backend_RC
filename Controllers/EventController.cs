using Backend_RC.DTO;
using Backend_RC.Models;
using Backend_RC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_RC.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Получить список всех мероприятий
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _eventService.GetAllEventsAsync();
        return Ok(events);
    }

    /// <summary>
    /// Поулчить конертное мероприятие по ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _eventService.GetEventAsync(id);
        if (ev == null) 
            return NotFound("Событие не найдено.");

        return Ok(ev);
    }

    /// <summary>
    /// Создать новое мероприятие
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventDto dto)
    {
        if (ModelState.IsValid) 
            return BadRequest(ModelState);

        var eventModel = new EventModel
        {
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Date = dto.Date,
            Time = dto.Time,
            Genre = dto.Genre,
            AgeRating = dto.AgeRating,
            ImageUrl = dto.ImageUrl,
            AdditionalInfo = dto.AdditionalInfo
        };

        var newEvent = await _eventService.CreateEventAsync(eventModel);
        return CreatedAtAction(nameof(Get), new { id = newEvent.Id }, newEvent);
    }

    /// <summary>
    /// Обновить существующее мероприятие
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventDto model)
    {
        if(id != model.Id)
            return BadRequest("ID в параметрах не совпадает с ID модели");

        var existing = await _eventService.GetEventAsync(id);
        if(existing == null)
            return NotFound("Событие не найдено");

        // Обновляем поля (можно добавить маппинг)
        existing.Title = model.Title;
        existing.Subtitle = model.Subtitle;
        existing.Date = model.Date;
        existing.Time = model.Time;
        existing.Genre = model.Genre;
        existing.AgeRating = model.AgeRating;
        existing.ImageUrl = model.ImageUrl;
        existing.AdditionalInfo = model.AdditionalInfo;

        await _eventService.UpdateEventAsync(existing);
        return Ok(existing);
    }

    /// <summary>
    /// Удалить мероприятие
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _eventService.GetEventAsync(id);
        if(ev == null)
            return NotFound("Событие не найдено");

        await _eventService.DeleteEventAsync(id);
        return Ok("Событие удалено");
    }
}
