using FluxoCaixa.Domain.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Configurations;

public abstract class AbstractConfiguration<E> : IEntityTypeConfiguration<E> where E : Entity
{
   private readonly string tableName;

   public AbstractConfiguration() => this.tableName = typeof(E).Name + "s";

   public AbstractConfiguration(string tableName) => this.tableName = tableName;

   public virtual void Configure(EntityTypeBuilder<E> builder)
   {
      builder.ToTable(this.tableName);

      builder.HasKey(u => u.Id);
      builder.Property(u => u.Id).ValueGeneratedOnAdd();
   }
}