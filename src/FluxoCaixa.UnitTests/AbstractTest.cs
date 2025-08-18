using FluxoCaixa.Domain;
using FluxoCaixa.Infrastructure;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace FluxoCaixa.UnitTests;

public abstract class AbstractTest : IDisposable
{
   protected IServiceScope scope;

   protected IUnitOfWork unitOfWork;

   protected IServiceProvider provider;

   [BeforeScenario]
   protected void Compose()
   {
      this.provider = new ServiceCollection()
         .AddScoped<LancamentoHandler>()
         .AddScoped<IUnitOfWork, UnitOfWork>()
         .AddSqlServerContext()
         .BuildServiceProvider();

      this.scope = this.provider.CreateScope();

      this.unitOfWork = this.provider
         .GetRequiredService<IUnitOfWork>();
   }

   [AfterScenario]
   public void Dispose()
   {
      // this.unitOfWork?.Clear();
      this.scope?.Dispose();
   }
}