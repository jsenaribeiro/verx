using FluxoCaixa.Domain.Lancamentos;
using FluxoCaixa.Domain.Usuarios;

namespace FluxoCaixa.Domain;

public interface IUnitOfWork
{
   IUsuarioRepository Usuarios { get; }

   ILancamentoRepository Lancamentos { get; }

   void Clear();

   Task BeginAsync();

   Task CommitAsync();

   Task RollbackAsync();  
}
