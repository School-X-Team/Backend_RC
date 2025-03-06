namespace Backend_RC.DTO;

/// <summary>
/// Ответ с информацией о виртуальной карте
/// </summary>
public class VCardResponseDto
{
    public required string CardNumber
    {
        get; set;
    }
    public bool IsConfirmed
    {
        get; set;
    }
    public DateTime CreatedAt
    {
        get; set;
    }
}
