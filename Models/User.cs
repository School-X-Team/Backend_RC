
using Microsoft.AspNetCore.Identity;

namespace Backend_RC.Models;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public VCardModel? VirtualCard { get; set; }
    public IndicatedCardModel? IndicatedCard { get; set; }
    public virtual ICollection<BonusPointModel> BonusPoints { get; set; } = new List<BonusPointModel>();
    public virtual ICollection<AchievementModel> Achievements { get; set; } = new List<AchievementModel>();
    public virtual ICollection<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();
    public virtual ICollection<UserSettingModel> UserSettings { get; set; } = new List<UserSettingModel>();
}