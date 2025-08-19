using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Lancamentos;

public interface ILancamentoRepository : IRepository<Lancamento>
{
   public Task<decimal> GetSaldoDiarioAsync(DateOnly data);
}