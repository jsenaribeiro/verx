using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoService
{
   private readonly IUnitOfWork _unitOfWork;

   public LancamentoService(IServiceProvider provider)
   {
      _unitOfWork = provider.GetService(typeof(IUnitOfWork)) as IUnitOfWork
         ?? throw new ArgumentNullException(nameof(IUnitOfWork));
   }

   public async Task<bool> Debitar(decimal valor, DateOnly data)
   {
      valor = valor > 0 ? valor * -1 : valor;

      var lancamento = new Lancamento(valor) { Data = data };

      await _unitOfWork.Lancamentos.SaveAsync(lancamento);

      return true;
   }

   public async Task<bool> Creditar(decimal valor, DateOnly data)
   {
      var lancamento = new Lancamento(valor) { Data = data };

      await _unitOfWork.Lancamentos.SaveAsync(lancamento);

      return true;
   }

   public async Task<decimal> SaldoDiario(DateOnly data) =>
      await _unitOfWork.Lancamentos.GetSaldoDiarioAsync(data);
}