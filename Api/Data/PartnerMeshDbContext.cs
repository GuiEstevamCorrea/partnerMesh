using System;
using System.Collections.Generic;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public partial class PartnerMeshDbContext : DbContext
{
    public PartnerMeshDbContext()
    {
    }

    public PartnerMeshDbContext(DbContextOptions<PartnerMeshDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<BusinessType> BusinessTypes { get; set; }

    public virtual DbSet<BussinessType> BussinessTypes { get; set; }

    public virtual DbSet<Comission> Comissions { get; set; }

    public virtual DbSet<ComissionPayment> ComissionPayments { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserVetor> UserVetors { get; set; }

    public virtual DbSet<Vetore> Vetores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => e.CreatedAt, "IX_AuditLogs_CreatedAt");

            entity.HasIndex(e => new { e.Entity, e.EntityId }, "IX_AuditLogs_Entity_EntityId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Datas).HasMaxLength(4000);
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasIndex(e => e.BussinessTypeId, "IX_Businesses_BussinessTypeId");

            entity.HasIndex(e => e.PartnerId, "IX_Businesses_PartnerId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Observations).HasMaxLength(1000);
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.BussinessType).WithMany(p => p.Businesses)
                .HasForeignKey(d => d.BussinessTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Partner).WithMany(p => p.Businesses)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BusinessType>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<BussinessType>(entity =>
        {
            entity.ToTable("BussinessType");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Comission>(entity =>
        {
            entity.HasIndex(e => e.BussinessId, "IX_Comissions_BussinessId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Bussiness).WithOne(p => p.Comission).HasForeignKey<Comission>(d => d.BussinessId);
        });

        modelBuilder.Entity<ComissionPayment>(entity =>
        {
            entity.ToTable("ComissionPayment");

            entity.HasIndex(e => e.ComissionId, "IX_ComissionPayment_ComissionId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Comission).WithMany(p => p.ComissionPayments).HasForeignKey(d => d.ComissionId);
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasIndex(e => e.RecommenderId, "IX_Partners_RecommenderId");

            entity.HasIndex(e => e.VetorId, "IX_Partners_VetorId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Recommender).WithMany(p => p.InverseRecommender).HasForeignKey(d => d.RecommenderId);

            entity.HasOne(d => d.Vetor).WithMany(p => p.Partners)
                .HasForeignKey(d => d.VetorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.Token, "IX_RefreshTokens_Token").IsUnique();

            entity.HasIndex(e => e.UserId, "IX_RefreshTokens_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<UserVetor>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.VetorId });

            entity.ToTable("UserVetor");

            entity.HasIndex(e => e.VetorId, "IX_UserVetor_VetorId");

            entity.HasOne(d => d.User).WithMany(p => p.UserVetors).HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Vetor).WithMany(p => p.UserVetors).HasForeignKey(d => d.VetorId);
        });

        modelBuilder.Entity<Vetore>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
