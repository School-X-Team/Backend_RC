//using System.ComponentModel.DataAnnotations;

//namespace Backend_RC.Models;
///// <summary>
///// Модель для билетов на мероприятия
///// </summary>
//public class TicketModel
//{
//    public string Id { get; set; }

//    [Required]
//    public string UserId { get; set; }

//    [Required]
//    public int EventId { get; set; }

//    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

//    //Навигационные свойства
//    public virtual EventModel Event { get; set; }

//    public virtual User User { get; set; }
//}
