using System;
using GuessMaster.Data.Models;
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

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<User> Users { get; set; }

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
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(GETDATE())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(GETDATE())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(GETDATE())")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime");

            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(u => u.GameSession)
                .WithMany(gs => gs.Users)
                .HasForeignKey(u => u.SessionId)
                .HasConstraintName("FK_User_GameSession");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
