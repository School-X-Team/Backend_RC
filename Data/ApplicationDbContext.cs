using Backend_RC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<VCardModel> VCards { get; set; }
    public DbSet<IndicatedCardModel> IndicatedCards { get; set; }
    public DbSet<EventModel> Events { get; set; }
    public DbSet<TicketModel> Tickets { get; set; }
    public DbSet<CulturalRoute> CulturalRoutes { get; set; }
    public DbSet<BonusPointModel> BonusPoints { get; set; }
    public DbSet<BannerModel> Banners { get; set; }
    public DbSet<AchievementModel> Achievements { get; set; }
    public DbSet<NewsItemModel> News { get; set; }
    public DbSet<TransportScheduleModel> TransportSchedule { get; set; }
    public DbSet<StoreItemModel> Store { get; set; }
    public DbSet<NotificationModel> Notifications { get; set; }
    public DbSet<UserSettingModel> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Связь один к одному: Пользователь -> Виртуальная карта
        builder.Entity<User>()
            .HasOne(u => u.VirtualCard)
            .WithOne(vc => vc.User)
            .HasForeignKey<VCardModel>(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один к одному: Пользователь -> Указанная карта
        builder.Entity<User>()
            .HasOne(u => u.IndicatedCard)
            .WithOne(ic => ic.User)
            .HasForeignKey<IndicatedCardModel>(ic => ic.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один ко многим: Пользователь -> Билеты
        builder.Entity<TicketModel>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один ко многим: Событие -> Билеты
        builder.Entity<TicketModel>()
            .HasOne(t => t.Event)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один ко многим: Пользователь -> Бонусные баллы
        builder.Entity<BonusPointModel>()
            .HasOne(bp => bp.User)
            .WithMany(u => u.BonusPoints)
            .HasForeignKey(bp => bp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один ко многим: Пользователь -> Достижения
        builder.Entity<AchievementModel>()
            .HasOne(a => a.User)
            .WithMany(u => u.Achievements)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один ко многим: Пользователь -> Уведомления
        builder.Entity<NotificationModel>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один ко многим: Пользователь -> Настройки
        builder.Entity<UserSettingModel>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSettings)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
