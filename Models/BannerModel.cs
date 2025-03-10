using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;

/// <summary>
/// Модель баннеров
/// </summary>
public class BannerModel
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [MaxLength(512)]
    public string ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
