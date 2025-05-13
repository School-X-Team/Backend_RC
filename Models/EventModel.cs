using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_RC.Models;

/// <summary>
/// Модель события (выставка, концерт и т. д.).
/// </summary>
public class EventModel
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Тип события (например, выставка, концерт)
    /// </summary>
    [Required]
    public string Type { get; set; }

    /// <summary>
    /// Название мероприятия
    /// </summary>
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// Дата начала мероприятия (например, 15 апреля)
    /// </summary>
    [Required]
    public string DateStart { get; set; }

    /// <summary>
    /// Дата окончания мероприятия (например, 16 апреля)
    /// </summary>
    [Required]
    public string DateEnd { get; set; }

    /// <summary>
    /// Время начала (например, 10:00)
    /// </summary>
    [Required]
    public string TimeStart { get; set; }

    /// <summary>
    /// Время окончания (например, 11:05)
    /// </summary>
    [Required]
    public string TimeEnd { get; set; }

    /// <summary>
    /// Длительность мероприятия (например, "1 ч 5 мин")
    /// </summary>
    public string TakeTime { get; set; }

    /// <summary>
    /// Вознаграждение за посещение (например, 4 балла)
    /// </summary>
    public string Reward { get; set; }

    /// <summary>
    /// Стоимость билета
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// Ссылка на изображение
    /// </summary>
    [Required]
    public string Image { get; set; }

    /// <summary>
    /// Описание мероприятия
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Географические координаты [широта, долгота]
    /// </summary>
    public double[] StartCoordinates { get; set; }
    public double[] EndCoordinates { get; set; }
}
