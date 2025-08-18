using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoHandler : Handler
   , IHandler<SaldoDiarioQuery, decimal>
   , IHandler<CreditarCommand, bool>
   , IHandler<DebitarCommand, bool>
{
   public LancamentoHandler(IServiceProvider provider): base(provider) { }

   public async Task<bool> HandleAsync(DebitarCommand command, CancellationToken cancel = default)
   {
      ValidarSeArgumentaoNaoEstaNulo(command);

      var lancamento = new Lancamento(command.valor * -1) { Data = command.Data };

      await unitOfWork.Lancamentos.SaveAsync(lancamento);

      return true;
   }

   public async Task<bool> HandleAsync(CreditarCommand command, CancellationToken cancel = default)
   {
      ValidarSeArgumentaoNaoEstaNulo(command);

      var lancamento = new Lancamento(command.valor) { Data = command.Data };

      await unitOfWork.Lancamentos.SaveAsync(lancamento);

      return true;
   }

   public async Task<decimal> HandleAsync(SaldoDiarioQuery query, CancellationToken cancel = default)
   {
      ValidarSeArgumentaoNaoEstaNulo(query);

      return await unitOfWork.Lancamentos.GetAsync(query);
   }

   private E ValidarSeArgumentaoNaoEstaNulo<E>(E handle) => handle is null
      ? throw new NullReferenceException(typeof(E).Name) : handle;
}