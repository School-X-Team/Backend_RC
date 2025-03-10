using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;
/// <summary>
/// Модель уведомлений в профиле пользователя
/// </summary>
public class NotificationModel
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool isRead { get; set; } = false;

    public virtual User User { get; set; }
}