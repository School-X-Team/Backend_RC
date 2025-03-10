using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;

/// <summary>
/// Модель культурных маршрутов
/// </summary>
public class CulturalRoute
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int DurationMinutes { get; set; }
}
