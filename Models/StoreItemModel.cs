using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;
/// <summary>
/// Модель магазина (товаров, которые можно приобрести за баллы)
/// </summary>
public class StoreItemModel
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string ItemName { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int PricePoints { get; set; }

    [Required]
    public int AvialableStock { get; set; }
}
