using FluxoCaixa.Domain;
using FluxoCaixa.Domain.Usuarios;
using FluxoCaixa.Domain.Lancamentos;
using FluxoCaixa.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure;

namespace FluxoCaixa.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
   private IServiceProvider _provider;

   public UnitOfWork(IServiceProvider provider)
   {
      _provider = provider;
      Usuarios = new UsuarioRepository(provider);
      Lancamentos = new LancamentoRepository(provider);
   }

   public IUsuarioRepository Usuarios { get; }

   public ILancamentoRepository Lancamentos { get; }

   public Task BeginAsync() => throw new NotImplementedException();

   public Task CommitAsync() => throw new NotImplementedException();

   public Task RollbackAsync() => throw new NotImplementedException();

   public void Clear()
   {
      try
      {
         var sqlServerContext = _provider
            .GetRequiredService<SqlServerContext>();

         if (sqlServerContext.Database is null) return;

         sqlServerContext.Database.EnsureDeleted();
         sqlServerContext.Database.EnsureCreated();
      }
      catch (Exception ex)
      {
         Console.WriteLine(ex.Message);
      }
   }
}
