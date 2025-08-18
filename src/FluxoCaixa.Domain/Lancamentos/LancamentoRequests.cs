using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Lancamentos;

public abstract record DataHoraRequest
{
   public DateOnly Data { get; set; } = DateOnly.FromDateTime(DateTime.Now);

   public TimeOnly Hora { get; set; } = TimeOnly.FromDateTime(DateTime.Now);

}

public partial record SaldoDiarioQuery(string data) : IQuery<decimal>;

public partial record CreditarCommand(decimal valor) : DataHoraRequest, ICommand<bool>;

public partial record DebitarCommand(decimal valor) : DataHoraRequest, ICommand<bool>;