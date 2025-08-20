using FluxoCaixa.Domain.Commons;
using MediatR;

namespace FluxoCaixa.Domain.Lancamentos;

public abstract record DataHoraRequest
{
   public DateOnly Data { get; set; } = DateOnly.FromDateTime(DateTime.Now);

   public TimeOnly Hora { get; set; } = TimeOnly.Parse(DateTime.Now.ToString("HH:mm:ss"));

}

public record SaldoDiarioQuery(string data) : IRequest<decimal>;

public record CreditarCommand(decimal valor) : DataHoraRequest, IRequest<bool>;

public record DebitarCommand(decimal valor) : DataHoraRequest, IRequest<bool>;