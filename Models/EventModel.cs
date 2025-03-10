using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_RC.Models;
/// <summary>
/// модель для мероприятий
/// </summary>
public class EventModel
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal Price { get; set; } = 0;

    public ICollection<TicketModel> Tickets { get; set; }
}
