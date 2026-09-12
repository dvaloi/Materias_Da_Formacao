using Beneficios.Domain.Entities;
using Beneficios.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beneficios.Infrastructure.Persistence;

public class BeneficiosDbContext(DbContextOptions<BeneficiosDbContext> options) : DbContext(options)
{
    public DbSet<PedidoBeneficio> Pedidos => Set<PedidoBeneficio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PedidoBeneficio>(entity =>
        {
            entity.ToTable("PedidosBeneficio");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.ValorSolicitado).HasPrecision(18, 2);
            entity.Property(p => p.Tipo)
                .HasConversion(new EnumToStringConverter<TipoBeneficio>())
                .HasMaxLength(20);
            entity.Property(p => p.Status)
                .HasConversion(new EnumToStringConverter<StatusPedido>())
                .HasMaxLength(20);
            entity.Property(p => p.MotivoRejeicao).HasMaxLength(500);
        });
    }
}
