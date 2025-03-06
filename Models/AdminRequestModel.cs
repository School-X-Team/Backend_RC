namespace Backend_RC.Models;

public class AdminRequestModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CardNumber { get; set; }
    public bool IsApproved { get; set; }
    public DateTime RequestedAt { get; set; }
}
