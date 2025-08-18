using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Infrastructure.Adapters;

public partial record SaldoDiarioQuery(string data) : IQuery<decimal>, IQueryRequest<decimal>;

public partial record CreditarCommand(decimal valor) : ICommand<bool>, ICommandRequest<bool>;

public partial record DebitarCommand(decimal valor) : ICommand<bool>, ICommandRequest<bool>;