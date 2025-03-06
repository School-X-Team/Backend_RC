namespace Backend_RC.DTO;

/// <summary>
/// Запрос на создание виртуальной карты
/// </summary>
public class VCardRequestDto
{
    public required string Email { get; set; }
    public required string PinCode { get; set; }
}
