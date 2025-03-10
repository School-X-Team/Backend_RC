using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;
/// <summary>
/// Модель расписания городского транспорта
/// </summary>
public class TransportScheduleModel
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string RouteName { get; set; }

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }
}
