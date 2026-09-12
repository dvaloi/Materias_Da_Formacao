using ProvaVida.Domain.Entities;
using ProvaVida.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProvaVida.Infrastructure.Persistence;

public class ProvaVidaDbContext(DbContextOptions<ProvaVidaDbContext> options) : DbContext(options)
{
    public DbSet<RegistroProvaVida> Registros => Set<RegistroProvaVida>();
    public DbSet<PerfilBiometrico> PerfisBiometricos => Set<PerfilBiometrico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistroProvaVida>(entity =>
        {
            entity.ToTable("RegistrosProvaVida");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Nuit).HasMaxLength(9).IsRequired();
            entity.Property(r => r.ScoreLiveness).HasPrecision(5, 2);
            entity.Property(r => r.FaceHash).HasMaxLength(128).IsRequired();
            entity.Property(r => r.Observacao).HasMaxLength(500);
            entity.Property(r => r.Status)
                .HasConversion(new EnumToStringConverter<StatusProvaVida>())
                .HasMaxLength(20);

            entity.HasIndex(r => r.ContribuinteId);
            entity.HasIndex(r => r.Nuit);
        });

        modelBuilder.Entity<PerfilBiometrico>(entity =>
        {
            entity.ToTable("PerfisBiometricos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nuit).HasMaxLength(9).IsRequired();
            entity.Property(p => p.FaceHashReferencia).HasMaxLength(128).IsRequired();
            entity.Property(p => p.FaceEmbeddingJson).HasColumnType("text");
            entity.HasIndex(p => p.ContribuinteId).IsUnique();
            entity.HasIndex(p => p.Nuit);
        });
    }
}
