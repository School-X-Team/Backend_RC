namespace Backend_RC.DTO;

public class AdminRequestDto
{
    public required int UserId { get; set; }
    public required string CardNumber { get; set; }
    public required bool IsConfirmed { get; set; }
}
