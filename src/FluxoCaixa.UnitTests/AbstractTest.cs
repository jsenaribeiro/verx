using FluxoCaixa.Domain;
using FluxoCaixa.Infrastructure;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.UnitTests;

public abstract class AbstractTest : IDisposable
{
   protected IServiceScope scope;

   protected IUnitOfWork unitOfWork;

   protected IServiceProvider provider;

   protected AbstractTest()
   {
      provider = Compose();
   }

   [BeforeScenario]
   protected IServiceProvider Compose()
   {
      this.provider = new ServiceCollection()
         .AddScoped<LancamentoHandler>()
         .AddScoped<IUnitOfWork, UnitOfWork>()
         .AddLogging(x => x.AddConsole())
         .AddMediatorCQRS()
         .AddSqlServerContext()
         .BuildServiceProvider();

      this.scope = this.provider.CreateScope();

      this.unitOfWork = this.provider
         .GetRequiredService<IUnitOfWork>();

      return this.provider;
   }

   [AfterScenario]
   public void Dispose() => scope?.Dispose();

   public T? GetValueOf<T>(IActionResult result)
   {
      var SEM_CONTEUDO = "Nao tem conteudo no resultado";
      var TIPO_ERRADO = $"Tipo esperado '{typeof(T).Name}' falhou";

      if (result is not ObjectResult resultado)
         throw new Exception(SEM_CONTEUDO);

      if (resultado.Value is not T valor)
         throw new Exception(TIPO_ERRADO);

      return valor;
   }
}