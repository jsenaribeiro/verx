namespace FluxoCaixa.Domain.Commons;
public interface IHandle<R> {}

public interface ICommand<R> : IHandle<R> { }

public interface IQuery<R> : IHandle<R> { }

public interface IHandler<H, R> where H : IHandle<R>
{
   Task<R> HandleAsync(H handle, CancellationToken cancel = default);
}