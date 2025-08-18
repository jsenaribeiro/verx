using FluxoCaixa.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Configurations;

public class UsuarioConfiguration : AbstractConfiguration<Usuario>
{
   public override void Configure(EntityTypeBuilder<Usuario> builder)
   {
      builder.Property(x => x.Email);

      builder.Property(x => x.Nome)
         .HasColumnType("varchar(99)");

      builder.Property(x => x.Senha)
         .HasColumnType("varchar(50)");
   }
}