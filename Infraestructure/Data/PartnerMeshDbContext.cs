using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data;

public class PartnerMeshDbContext : DbContext
{
    public PartnerMeshDbContext(DbContextOptions<PartnerMeshDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Vetor> Vetores => Set<Vetor>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<BusinessType> BusinessTypes => Set<BusinessType>();
    public DbSet<Bussiness> Businesses => Set<Bussiness>();
    public DbSet<Comission> Comissions => Set<Comission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Permission).IsRequired();
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // Configuração da tabela associativa UserVetor
            entity.HasMany(u => u.UserVetores)
                .WithOne(uv => uv.User)
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserVetor Configuration (Tabela Associativa)
        modelBuilder.Entity<UserVetor>(entity =>
        {
            entity.HasKey(uv => new { uv.UserId, uv.VetorId });

            entity.HasOne(uv => uv.User)
                .WithMany(u => u.UserVetores)
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uv => uv.Vetor)
                .WithMany(v => v.UserVetores)
                .HasForeignKey(uv => uv.VetorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(uv => uv.Active).IsRequired();
        });

        // Vetor Configuration
        modelBuilder.Entity<Vetor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasMany(v => v.Partners)
                .WithOne(p => p.Vetor)
                .HasForeignKey(p => p.VetorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(v => v.UserVetores)
                .WithOne(uv => uv.Vetor)
                .HasForeignKey(uv => uv.VetorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Partner Configuration
        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // Auto-referência (Recomendador)
            entity.HasOne(p => p.Recommender)
                .WithMany(p => p.Recommended)
                .HasForeignKey(p => p.RecommenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação com Vetor
            entity.HasOne(p => p.Vetor)
                .WithMany(v => v.Partners)
                .HasForeignKey(p => p.VetorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BusinessType Configuration
        modelBuilder.Entity<BusinessType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();
        });

        // Business Configuration
        modelBuilder.Entity<Bussiness>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Observations).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Relação com Partner
            entity.HasOne(b => b.Partner)
                .WithMany()
                .HasForeignKey(b => b.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação com BusinessType
            entity.HasOne(b => b.BussinessType)
                .WithMany()
                .HasForeignKey(b => b.BussinessTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação 1:1 com Comission
            entity.HasOne(b => b.Comissao)
                .WithOne(c => c.Bussiness)
                .HasForeignKey<Comission>(c => c.BussinessId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Comission Configuration
        modelBuilder.Entity<Comission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalValue).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Relação 1:1 com Business
            entity.HasOne(c => c.Bussiness)
                .WithOne(b => b.Comissao)
                .HasForeignKey<Comission>(c => c.BussinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação 1:N com ComissionPayment
            entity.HasMany(c => c.Pagamentos)
                .WithOne(cp => cp.Comission)
                .HasForeignKey(cp => cp.ComissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ComissionPayment Configuration (Owned Type)
        modelBuilder.Entity<ComissionPayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.TipoPagamento).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PartnerId).IsRequired();

            entity.HasOne(cp => cp.Comission)
                .WithMany(c => c.Pagamentos)
                .HasForeignKey(cp => cp.ComissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Action).IsRequired();
            entity.Property(e => e.Entity).IsRequired();
            entity.Property(e => e.EntityId).IsRequired();
            entity.Property(e => e.Datas).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.Entity, e.EntityId });
        });

        // RefreshToken Configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsRevoked).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired();

            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
