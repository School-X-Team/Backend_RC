using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;

public class BonusPointModel
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public int Points { get; set; } =0;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; }
}
