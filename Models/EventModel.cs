using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_RC.Models;
/// <summary>
/// модель события (концерт, спектакль и т. д.) для афиши.
/// </summary>
public class EventModel
{
    public int Id { get; set; }

    /// <summary>
    /// Название мероприятия
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Подзаголовок или краткое описание
    /// </summary>
    public string? Subtitle { get; set; }
    /// <summary>
    /// Дата проведения (например, 3 июня)
    /// </summary>
    public DateTime Date{ get; set; }
    /// <summary>
    /// Время проведения (например, 19:00)
    /// </summary>
    public string Time { get; set; }
    /// <summary>
    /// Жанр (например, джаз, классика)
    /// </summary>
    public string Genre { get; set; }
    /// <summary>
    /// Возрастное ограничение (например, 6+).
    /// </summary>
    public string AgeRating { get; set; }
    /// <summary>
    /// URL-адрес изображения для афиши.
    /// </summary>
    public string ImageUrl { get; set; }
    /// <summary>
    /// Дополнительный текст (например, «Посвящение Фрэнку Синатре»).
    /// </summary>
    public string? AdditionalInfo { get; set; }

    public ICollection<TicketModel> Tickets { get; set; }
}
