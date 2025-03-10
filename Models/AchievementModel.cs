using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;

/// <summary>
/// Модель достижений пользователя
/// </summary>
public class AchievementModel
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; }
}
