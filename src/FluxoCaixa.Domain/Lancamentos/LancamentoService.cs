using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoService
{
   private readonly IUnitOfWork _unitOfWork;

   private readonly Guid _usuarioId;

   public LancamentoService(IServiceProvider provider, Guid usuarioId)
   {
      _usuarioId = usuarioId;
      _unitOfWork = provider.GetService(typeof(IUnitOfWork)) as IUnitOfWork
         ?? throw new ArgumentNullException(nameof(IUnitOfWork));
   }

   public async Task<bool> Debitar(decimal valor, DateOnly data) =>
      await Transacao(valor > 0 ? valor * -1 : valor, data);

   public async Task<bool> Creditar(decimal valor, DateOnly data) =>
      await Transacao(valor, data);

   private async Task<bool> Transacao(decimal valor, DateOnly data)
   {
      await _unitOfWork.Lancamentos.SaveAsync(new Lancamento(valor)
      {
         Data = data,
         UsuarioId = _usuarioId
      });

      return true;
   }

   public async Task<decimal> SaldoDiario(DateOnly data) =>
      await _unitOfWork.Lancamentos.GetSaldoDiarioAsync(data);
}