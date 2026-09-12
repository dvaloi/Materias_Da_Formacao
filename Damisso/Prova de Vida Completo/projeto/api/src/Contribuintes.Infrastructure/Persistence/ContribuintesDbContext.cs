using Contribuintes.Domain.Entities;
using Contribuintes.Domain.Enums;
using Contribuintes.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Contribuintes.Infrastructure.Persistence;

public class ContribuintesDbContext(DbContextOptions<ContribuintesDbContext> options) : DbContext(options)
{
    public DbSet<Contribuinte> Contribuintes => Set<Contribuinte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contribuinte>(entity =>
        {
            entity.ToTable("Contribuintes");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nuit)
                .HasConversion(
                    n => n.Valor,
                    v => Nuit.Criar(v))
                .HasColumnName("Nuit")
                .HasMaxLength(9);

            entity.Property(c => c.Nome).HasMaxLength(200).IsRequired();
            entity.Property(c => c.SalarioMensal).HasPrecision(18, 2);
            entity.Property(c => c.Situacao)
                .HasConversion(new EnumToStringConverter<SituacaoContribuinte>())
                .HasMaxLength(20);
            entity.Property(c => c.Perfil)
                .HasConversion(new EnumToStringConverter<PerfilContribuinte>())
                .HasMaxLength(30);

            entity.HasIndex(c => c.Nuit).IsUnique();
        });
    }
}
