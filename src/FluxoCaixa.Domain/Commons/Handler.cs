namespace FluxoCaixa.Domain.Commons;

public abstract class Handler
{
   protected readonly IUnitOfWork unitOfWork;

   protected Handler(IServiceProvider provider)
   {
      unitOfWork = provider.GetService(typeof(IUnitOfWork)) as IUnitOfWork
         ?? throw new ArgumentNullException(nameof(IUnitOfWork));
   }
}