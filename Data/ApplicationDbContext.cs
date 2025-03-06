using Backend_RC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<VCardModel> VCards { get; set; }
    public DbSet<IndicatedCardModel> IndicatedCards { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasOne(u => u.VirtualCard)
            .WithOne(vc => vc.User)
            .HasForeignKey<VCardModel>(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<User>()
            .HasOne(u=>u.IndicatedCard)
            .WithOne(ic=>ic.User)
            .HasForeignKey<IndicatedCardModel>(ic => ic.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
