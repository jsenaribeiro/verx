using FluxoCaixa.Domain;
using FluxoCaixa.Infrastructure;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FluxoCaixa.Domain.Usuarios;
using Microsoft.Extensions.Configuration;

namespace FluxoCaixa.UnitTests;

public abstract class AbstractTest : IDisposable
{
   protected IServiceScope scope;

   protected IUnitOfWork unitOfWork;

   protected IServiceProvider provider;

   protected DefaultHttpContext httpContext;

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
         .AddSingleton(AddHttpContext)
         .AddSingleton(TestConfiguration)
         .AddMediatorCQRS()
         .AddSqlServerContext()
         .BuildServiceProvider();

      this.scope = this.provider.CreateScope();

      this.unitOfWork = this.provider
         .GetRequiredService<IUnitOfWork>();

      return this.provider;
   }

   private Func<IServiceProvider, IHttpContextAccessor> AddHttpContext =>
      (IServiceProvider sp) => new HttpContextAccessor { HttpContext = httpContext };

   [AfterScenario]
   public void Dispose() => scope?.Dispose();

   protected T? GetValueOf<T>(IActionResult result)
   {
      var SEM_CONTEUDO = "Nao tem conteudo no resultado";
      var TIPO_ERRADO = $"Tipo esperado '{typeof(T).Name}' falhou";

      if (result is not ObjectResult resultado)
         throw new Exception(SEM_CONTEUDO);

      if (resultado.Value is not T valor)
         throw new Exception(TIPO_ERRADO);

      return valor;
   }

   protected ControllerContext UsuarioLogado(Usuario usuario)
   {
      unitOfWork.Usuarios.SaveAsync(usuario).Wait();

      var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
      {
         new Claim(ClaimTypes.Email, usuario.Email)
      }));

      httpContext = new DefaultHttpContext { User = claims };
      httpContext.Request.Headers["Authorization"] = "Bearer fake";

      return new ControllerContext { HttpContext = httpContext };
   }

   private IConfiguration TestConfiguration
   {
      get
      {
         var configuration = new Dictionary<string, string>
         {
            {"AllowedHosts", "*"},
            {"Logging:LogLevel:Default", "Information"},
            {"Logging:LogLevel:Microsoft.AspNetCore", "Warning"},
            {"Jwt:Key", "d2j9uV6pQ5r7s8t0u1v2w3x4y5z6a7b8c9d0e1f2g3h4i5j6k7l8m9n0o1p2q3r"},
            {"Jwt:Issuer", "fluxo-caixa-issuer"},
            {"Jwt:Audience", "fluxo-caixa-audience"}
         };

         return new ConfigurationBuilder()
             .AddInMemoryCollection(configuration!)
             .Build();
      }
   }
}