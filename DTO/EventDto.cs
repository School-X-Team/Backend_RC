using System.ComponentModel.DataAnnotations;

namespace Backend_RC.DTO;
/// <summary>
/// DTO для отображения данных события (афиши)
/// </summary>
public class EventDto
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Название события
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Название должно содержать от 5 до 100 символов.")]
    public string Title { get; set; }

    /// <summary>
    /// Подзаголовок или краткое описание
    /// </summary>
    [StringLength(200, ErrorMessage = "Подзаголовок не может превышать 200 символов.")]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Дата проведения события
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Время проведения события (формат HH:mm)
    /// </summary>
    [Required]
    [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Время должно быть в формате HH:mm")]
    public string Time { get; set; }

    /// <summary>
    /// Жанр события (например, джаз, классика)
    /// </summary>
    [Required]
    public string Genre { get; set; }

    /// <summary>
    /// Возрастное ограничение (например, 6+)
    /// </summary>
    [Required]
    public string AgeRating { get; set; }

    /// <summary>
    /// URL изображения для афиши
    /// </summary>
    [Required]
    [Url(ErrorMessage = "Некорректный URL-адрес изображения.")]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Дополнительная информация (например, особенности или описание)
    /// </summary>
    public string? AdditionalInfo { get; set; }
}
