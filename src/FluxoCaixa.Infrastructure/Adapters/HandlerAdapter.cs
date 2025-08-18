using FluxoCaixa.Domain.Commons;
using MediatR;

namespace FluxoCaixa.Infrastructure.Adapters;

public interface ICommandRequest<R> : IRequest<R>, ICommand<R> { }

public interface IQueryRequest<R> : IRequest<R>, IQuery<R> { }

public class CommandHandlerAdapter<C, R> : IRequestHandler<C, R> where C : ICommand<R>, IRequest<R>
{
   private readonly IHandler<C, R> _handler;

   public CommandHandlerAdapter(IHandler<C, R> handler) => _handler = handler;

   public Task<R> Handle(C request, CancellationToken cancel) =>
      _handler.HandleAsync(request, cancel);
}

public class QueryHandlerAdapter<Q, R> : IRequestHandler<Q, R> where Q : IQuery<R>, IRequest<R>
{
   private readonly IHandler<Q, R> _handler;

   public QueryHandlerAdapter(IHandler<Q, R> handler) => _handler = handler;

   public Task<R> Handle(Q request, CancellationToken cancel) =>
      _handler.HandleAsync(request, cancel);
}