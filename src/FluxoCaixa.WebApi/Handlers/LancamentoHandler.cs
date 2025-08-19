using FluxoCaixa.Domain.Commons;
using MediatR;

namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoHandler : AbstractHandler
   , IRequestHandler<SaldoDiarioQuery, decimal>
   , IRequestHandler<CreditarCommand, bool>
   , IRequestHandler<DebitarCommand, bool>
{
   public LancamentoHandler(IServiceProvider provider): base(provider) { }

   public Task<bool> Handle(DebitarCommand command, CancellationToken cancel) =>
      new LancamentoService(provider).Debitar(command.valor, command.Data);

   public Task<bool> Handle(CreditarCommand command, CancellationToken cancel) =>
      new LancamentoService(provider).Creditar(command.valor, command.Data);

   public Task<decimal> Handle(SaldoDiarioQuery query, CancellationToken cancel)
   {
      if (!DateOnly.TryParse(query.data, out var data))
         throw new ArgumentException($"Data inválida em {query.data}");

      return new LancamentoService(provider).SaldoDiario(data);
   }
}