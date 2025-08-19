using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;

namespace FluxoCaixa.Domain.Lancamentos;

public class Lancamento : Entity
{
   public const string VALOR_LANCAMENTO_ERRO = "Valor de lançamento não pode ser zero.";

   public Lancamento() { }

   public Lancamento(decimal valor)
   {
      if (valor == 0) throw new ArgumentException(VALOR_LANCAMENTO_ERRO);

      this.Data = DateOnly.FromDateTime(DateTime.UtcNow);
      this.Hora = TimeOnly.FromDateTime(DateTime.UtcNow);
      this.Valor = valor;
   }

   public DateOnly Data { get; set; }

   public TimeOnly Hora { get; set; }

   public decimal Valor { get; set; }

   public Usuario Usuario { get; set; }

   public Guid UsuarioId { get; set; }

   public LancamentoTipo Tipo =>
        Valor > 0 ? LancamentoTipo.Credito
      : Valor < 0 ? LancamentoTipo.Debito
      : LancamentoTipo.Default;
      
}