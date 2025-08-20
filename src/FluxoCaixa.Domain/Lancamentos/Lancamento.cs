using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;

namespace FluxoCaixa.Domain.Lancamentos;

public class Lancamento : Entity
{
   public Lancamento() { }

   public Lancamento(decimal valor)
   {
      this.Data = DateOnly.FromDateTime(DateTime.UtcNow);
      this.Hora = TimeOnly.FromDateTime(DateTime.UtcNow);
      this.Valor = valor;
   }

   public DateOnly Data { get; set; } = DateOnly.MinValue;

   public TimeOnly Hora { get; set; } = TimeOnly.FromTimeSpan(TimeSpan.Zero);

   public decimal Valor { get; set; }

   public Guid UsuarioId { get; set; } = Guid.Empty;

   public Usuario? Usuario { get; set; }

   public LancamentoTipo Tipo =>
        Valor > 0 ? LancamentoTipo.Credito
      : Valor < 0 ? LancamentoTipo.Debito
      : LancamentoTipo.Default;
      
}