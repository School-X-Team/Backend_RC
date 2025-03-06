using Backend_RC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_RC.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IndicatedCardRepository _cardRepository;

    public AdminController(IndicatedCardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    /// <summary>
    /// Получение списка заявок на подтверждение
    /// </summary>
    /// <returns></returns>
    [HttpGet("requests")]
    public async Task<IActionResult> Requests()
    {
        var pendingCards = await _cardRepository.GetPendingRequestsAsync();
        return Ok(pendingCards);
    }

    /// <summary>
    /// Подтверждение карты
    /// </summary>
    /// <returns></returns>
    [HttpPost("confirm/{requestId}")]
    public async Task<IActionResult> Confirm(string requestId)
    {
        var card = await _cardRepository.GetCardByUserIdAsync(requestId);

        if (card == null) 
            return NotFound("Заявка не найдена");

        if (card.isConfirmed)
            return BadRequest("Заявка уже подтверждена");

        card.isConfirmed = true;
        await _cardRepository.UpdateRequestAsync(card);

        return Ok("Карта успешно подтверждена");
    }
}
