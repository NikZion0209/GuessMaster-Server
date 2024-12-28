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

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomAssignment> RoomAssignments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Name=ApplicationDbContextConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__GameSess__C9F49290D38320B1");

            entity.Property(e => e.EndedAt).HasColumnType("datetime");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Room).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__GameSessi__RoomI__440B1D61");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__32863939D09C3AE3");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsFull).HasDefaultValue(false);
            entity.Property(e => e.PlayerCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<RoomAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__RoomAssi__32499E77A28DC477");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomAssignments)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__RoomAssig__RoomI__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.RoomAssignments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RoomAssig__UserI__403A8C7D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C7E994DAE");

            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
