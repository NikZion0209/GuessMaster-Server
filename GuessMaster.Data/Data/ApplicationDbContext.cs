using System;
using GuessMaster.Data.Models;
using GuessMaster.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace GuessMaster.Data.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Leaderboards> Leaderboards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ApplicationDbContextConnection");

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<User>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.Now;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(GETDATE())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(GETDATE())")
                .HasColumnType("datetime");

            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.AvatarId).IsRequired();
        });

        modelBuilder.Entity<Leaderboards>(entity =>
        {
            entity.HasKey(e => e.LeaderBoardRecordId);
            entity.Property(e => e.Gamemode).IsRequired();
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.AvatarId).IsRequired();
            entity.Property(e => e.Score).IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
