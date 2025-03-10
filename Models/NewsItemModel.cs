using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;

/// <summary>
/// Модель новостей (Афиша новостей и мероприятий)
/// </summary>
public class NewsItemModel
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
