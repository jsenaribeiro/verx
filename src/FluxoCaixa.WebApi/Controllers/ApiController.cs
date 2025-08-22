namespace FluxoCaixa.WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Polly;
using Polly.CircuitBreaker;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

public abstract class ApiController<E> : ControllerBase where E : class
{
   protected readonly ILogger<E> logger;

   protected readonly IMediator mediator;

   protected readonly IAsyncPolicy<HttpResponseMessage> policy;

   public ApiController(IServiceProvider provider)
   {
      logger = provider.GetRequiredService<ILogger<E>>();
      mediator = provider.GetRequiredService<IMediator>();
      policy = provider.GetRequiredService<IAsyncPolicy<HttpResponseMessage>>();
   }

   protected async Task<IActionResult> TryPolicyAsync<T>(Func<Task<T>> task)
   {
      var response = await policy.ExecuteAsync(async () =>
      {
         var resultado = default(T);

         if (await TryAsync(task) is OkObjectResult oor)
            resultado = (T?) Convert.ChangeType(oor.Value, typeof(T));

         var value = resultado is null ? "null"
            : resultado.GetType().IsPrimitive
            ? resultado.ToString() ?? string.Empty
            : JsonSerializer.Serialize(resultado);

         var content = new StringContent(value, Encoding.UTF8, "application/json");

         return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
      });

      if (response.IsSuccessStatusCode == false)
      {
         var mensagem = await response.Content.ReadAsStringAsync();
         return StatusCode((int)response.StatusCode, mensagem);
      }

      var code = (int)response.StatusCode;
      var json = await response.Content.ReadFromJsonAsync<T>();

      return StatusCode(code, json);
   }
   
   protected async Task<IActionResult> TryAsync<T>(Func<Task<T>> task)
   {
      try
      {
         return Ok(await task());
      }
      catch (UnauthorizedAccessException ex)
      {
         logger.LogError(ex, ex.Message);
         return Unauthorized(ex.Message);
      }
      catch (AuthenticationException ex)
      {
         logger.LogError(ex, ex.Message);
         return Unauthorized(ex.Message);
      }
      catch (BrokenCircuitException ex)
      {
         logger.LogError("O circuito está aberto. A requisição foi cancelada.");
         return StatusCode(503, ex.Message);
      }
      catch (NullReferenceException ex)
      {
         logger.LogError(ex, ex.Message);
         return NotFound(ex.Message);
      }
      catch (ArgumentException ex)
      {
         logger.LogError(ex, ex.Message);
         return BadRequest(ex.Message);
      }
      catch (Exception ex)
      {
         logger.LogError(ex, "Erro inesperado");
         return StatusCode(500, "Erro inesperado");
      }
   }
}