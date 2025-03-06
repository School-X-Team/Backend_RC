namespace Backend_RC.DTO;

/// <summary>
/// Запрос на привязку пластиковой карты
/// </summary>
public class IndicatedCardRequestDto
{
    public required string Email { get; set; }
    public required string PinCode { get; set; }
    public required string CardNumber { get; set; }
}
