using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Configurations;

public class LancamentoConfiguration : AbstractConfiguration<Lancamento>
{
   public override void Configure(EntityTypeBuilder<Lancamento> builder)
   {
      builder.Property(x => x.Data);

      builder.Property(x => x.Hora);

      builder.Property(x => x.Valor)
         .HasColumnType("decimal(18,2)")
         .HasDefaultValue(0);

      builder.Ignore(x => x.Tipo);
   }
}