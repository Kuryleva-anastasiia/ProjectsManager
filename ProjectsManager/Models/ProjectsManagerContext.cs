using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

public partial class ProjectsManagerContext : DbContext
{
    public ProjectsManagerContext()
    {
    }

    public ProjectsManagerContext(DbContextOptions<ProjectsManagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<TeamsProject> TeamsProjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\College\\Дипломы\\ProjectsManager\\ProjectsManager\\App_data\\ProjectsManager.mdf;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Priority__3214EC07C5140BF9");

            entity.Property(e => e.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07DB4DBA73");

            entity.Property(e => e.Description).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.User).WithMany(p => p.Projects).HasConstraintName("FK_Projects_ToUsers");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3214EC07C989301E");

            entity.Property(e => e.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Table");

            entity.Property(e => e.Description).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Title).UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.Tasks).HasConstraintName("FK_Tasks_ToUsers");

            entity.HasOne(d => d.Priority).WithMany(p => p.Tasks).HasConstraintName("FK_Tasks_ToPriority");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks).HasConstraintName("FK_Tasks_ToProjects");

            entity.HasOne(d => d.Status).WithMany(p => p.Tasks).HasConstraintName("FK_Tasks_ToStatus");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teams__3214EC078C4AADF2");

            entity.Property(e => e.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TeamMember");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers).HasConstraintName("FK_TeamMembers_ToTeams");

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers).HasConstraintName("FK_TeamMembers_ToUsers");
        });

        modelBuilder.Entity<TeamsProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeamsPro__3214EC079A8BFBAC");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Project).WithMany(p => p.TeamsProjects).HasConstraintName("FK_TeamsProjects_ToProjects");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamsProjects).HasConstraintName("FK_TeamsProjects_ToTeams");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Table__3214EC0797687DAD");

            entity.Property(e => e.LastName).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Login).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Patronymic).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Phone).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Role).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
