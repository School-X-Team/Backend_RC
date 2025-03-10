using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;
/// <summary>
/// Модель настроек пользователя
/// </summary>
public class UserSettingModel
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required, MaxLength(255)]
    public string SettingKey { get; set; }

    [Required]
    public string SettingValue { get; set; }

    public virtual User User { get; set; }
}
