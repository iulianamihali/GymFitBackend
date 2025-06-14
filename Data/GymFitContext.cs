using System;
using System.Collections.Generic;
using GymFit.Models;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Data;

public partial class GymFitContext : DbContext
{
    public GymFitContext()
    {
    }

    public GymFitContext(DbContextOptions<GymFitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientCourseEnrollment> ClientCourseEnrollments { get; set; }

    public virtual DbSet<ClientTrainerEnrollment> ClientTrainerEnrollments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<TrainingSession> TrainingSessions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Client__1788CC4C95027EC2");

            entity.ToTable("Client");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Goal)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.HealthNotes).HasColumnType("text");

            entity.HasOne(d => d.User).WithOne(p => p.Client)
                .HasForeignKey<Client>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Client__UserId__398D8EEE");
        });

        modelBuilder.Entity<ClientCourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientCo__3214EC0701F0DE82");

            entity.ToTable("ClientCourseEnrollment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EnrollmentDate).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.ClientCourseEnrollments)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__ClientCou__Clien__4F7CD00D");

            entity.HasOne(d => d.Course).WithMany(p => p.ClientCourseEnrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__ClientCou__Cours__5070F446");
        });

        modelBuilder.Entity<ClientTrainerEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientTr__3214EC0746E15755");

            entity.ToTable("ClientTrainerEnrollment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.ClientTrainerEnrollments)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__ClientTra__Clien__48CFD27E");

            entity.HasOne(d => d.Trainer).WithMany(p => p.ClientTrainerEnrollments)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK__ClientTra__Train__49C3F6B7");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC07F405F583");

            entity.ToTable("Course");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Active).HasColumnType("bit");
            entity.Property(e => e.Title)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Trainer).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK__Course__TrainerI__4CA06362");
           
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC07FD612839");

            entity.ToTable("Review");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Review__ClientId__44FF419A");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK__Review__TrainerI__45F365D3");
        });

        modelBuilder.Entity<TrainingSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Training__3214EC070473A65E");

            entity.ToTable("TrainingSession");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            
            entity.Property(e => e.Notes).HasColumnType("text");

            entity.HasOne(e => e.Client).WithMany(e => e.TrainingSessions)
                .HasForeignKey(e => e.ClientId)
                .HasConstraintName("FK__TrainingS__Clien__60A75C0F");

            entity.HasOne(e => e.Trainer).WithMany(e => e.TrainingSessions)
                .HasForeignKey(e => e.TrainerId)
                .HasConstraintName("FK__TrainingS__Train__619B8048");
        });


        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC076F6FDF40");

            entity.ToTable("Subscription");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ActivationDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Subscript__UserI__4222D4EF");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Trainer__1788CC4C228F354A");

            entity.ToTable("Trainer");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Certification)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PricePerHour).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Specialization)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.Trainer)
                .HasForeignKey<Trainer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Trainer__UserId__3C69FB99");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07E113E137");

            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
