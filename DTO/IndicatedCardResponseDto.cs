namespace Backend_RC.DTO;

/// <summary>
/// Ответ с информацией о пластиковой карте
/// </summary>
public class IndicatedCardResponseDto
{
    public required string CardNumber { get; set; }
    public bool isConfirmed { get; set; }
    public DateTime LinkedAt { get; set; }
}
