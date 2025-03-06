
using Microsoft.AspNetCore.Identity;

namespace Backend_RC.Models;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public VCardModel? VirtualCard { get; set; }
    public IndicatedCardModel? IndicatedCard { get; set; }
}