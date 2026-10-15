using CalorieTrack.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalorieTrack.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; }

    public DbSet<Food> Foods => Set<Food>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>()
            .HasOne(profile => profile.User)
            .WithOne()
            .HasForeignKey<UserProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Food>(entity =>
                {
                entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

                entity.Property(x => x.Brand)
                .HasMaxLength(200);

                entity.Property(x => x.ExternalId)
                .HasMaxLength(100);

                entity.HasIndex(x => x.ExternalId);
                });
    }
}
