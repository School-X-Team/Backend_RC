using System.Globalization;
using System.Reflection.Emit;
using System.Text.Json;
using Backend_RC.Models;
using Elastic.CommonSchema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



public class ApplicationDbContext : IdentityDbContext<Backend_RC.Models.User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<VCardModel> VCards { get; set; }
    public DbSet<IndicatedCardModel> IndicatedCards { get; set; }
    public DbSet<EventModel> Events { get; set; }
    public DbSet<CulturalRoute> CulturalRoutes { get; set; }
    public DbSet<BonusPointModel> BonusPoints { get; set; }
    public DbSet<BannerModel> Banners { get; set; }
    public DbSet<AchievementModel> Achievements { get; set; }
    public DbSet<NewsItemModel> News { get; set; }
    public DbSet<TransportScheduleModel> TransportSchedule { get; set; }
    public DbSet<StoreItemModel> Store { get; set; }
    public DbSet<NotificationModel> Notifications { get; set; }
    public DbSet<UserSettingModel> UserSettings { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Backend_RC.Models.Routee> Routes { get; set; }
    public DbSet<Tickets> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<EventModel>(builder =>
        {
            // Указываем тип столбца в Postgres
            builder.Property(r => r.StartCoordinates)
                   .HasColumnType("double precision[]");

            builder.Property(r => r.EndCoordinates)
                   .HasColumnType("double precision[]");
        });

        builder.Entity<TaskItem>(builder =>
        {
            // Указываем тип столбца в Postgres
            builder.Property(r => r.StartCoordinates)
                   .HasColumnType("double precision[]");

            builder.Property(r => r.EndCoordinates)
                   .HasColumnType("double precision[]");
        });

        builder.Entity<Routee>(builder =>
        {
            // Указываем тип столбца в Postgres
            builder.Property(r => r.StartCoordinates)
                   .HasColumnType("double precision[]");

            builder.Property(r => r.EndCoordinates)
                   .HasColumnType("double precision[]");
        });

        // Связь один к одному: Пользователь -> Виртуальная карта
        builder.Entity<Backend_RC.Models.User>()
            .HasOne(u => u.VirtualCard)
            .WithOne(vc => vc.User)
            .HasForeignKey<VCardModel>(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь один к одному: Пользователь -> Указанная карта
        builder.Entity<Backend_RC.Models.User>()
            .HasOne(u => u.IndicatedCard)
            .WithOne(ic => ic.User)
            .HasForeignKey<IndicatedCardModel>(ic => ic.UserId)
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
