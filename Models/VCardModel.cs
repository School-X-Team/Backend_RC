namespace Backend_RC.Models;

public class VCardModel
{
    public int Id { get; set; }
    public string CardNumber { get; set; }
    public bool isConfirmed { get; set; } = false;
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
}
