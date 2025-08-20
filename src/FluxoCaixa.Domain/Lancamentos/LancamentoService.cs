using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoService
{
   private readonly IUnitOfWork _unitOfWork;

   private readonly Guid _usuarioId;

   public const string VALOR_ZERO = "Valor não pode ser zero.";

   public const string VALOR_NEGATIVO = "Valor não pode ser negativo.";

   public LancamentoService(IServiceProvider provider, Guid usuarioId)
   {
      _usuarioId = usuarioId;
      _unitOfWork = provider.GetService(typeof(IUnitOfWork)) as IUnitOfWork
         ?? throw new ArgumentNullException(nameof(IUnitOfWork));
   }

   public async Task<bool> Debitar(decimal valor, DateOnly data) => await Transacao(valor, data, true);

   public async Task<bool> Creditar(decimal valor, DateOnly data) => await Transacao(valor, data);

   private async Task<bool> Transacao(decimal valor, DateOnly data, bool isCredito = false)
   {
      if (valor == 0) throw new ArgumentException(VALOR_ZERO);

      if (valor < 0) throw new ArgumentException(VALOR_NEGATIVO);

      if (isCredito && valor > 0) valor = valor * -1;

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