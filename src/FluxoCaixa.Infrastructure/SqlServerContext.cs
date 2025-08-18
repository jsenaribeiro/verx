namespace Infrastructure;

using System.Reflection;
using FluxoCaixa.Domain.Lancamentos;
using FluxoCaixa.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class SqlServerContext : DbContext
{
   public DbSet<Usuario> Usuarios { get; set; }

   public DbSet<Lancamento> Lancamentos { get; set; }

   public SqlServerContext(DbContextOptions<SqlServerContext> dco) : base(dco) {}

   protected override void OnModelCreating(ModelBuilder builder)
   {
      var currentAssembly = Assembly.GetExecutingAssembly();

      builder.ApplyConfigurationsFromAssembly(currentAssembly);

      base.OnModelCreating(builder);
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      if (optionsBuilder.IsConfigured) return;

      else optionsBuilder
         .EnableSensitiveDataLogging()
         .LogTo(Console.WriteLine, LogLevel.Information);
   }
}