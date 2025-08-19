using FluxoCaixa.Domain.Lancamentos;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Infrastructure.Repositories;

public class LancamentoRepository : AbstractRepository<Lancamento>, ILancamentoRepository
{
   public LancamentoRepository(IServiceProvider provider) : base(provider) { }

   public async Task<decimal> GetSaldoDiarioAsync(DateOnly data)
   {
      var lista = await Query(x => x.Data == data).ToListAsync();

      return lista.Sum(x => x.Valor);
   }
}