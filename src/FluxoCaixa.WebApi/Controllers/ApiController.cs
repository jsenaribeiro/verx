namespace FluxoCaixa.WebApi.Controllers;

using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;

public abstract class ApiController<E> : ControllerBase where E : class
{
   protected readonly ILogger<E> logger;

   public ApiController(IServiceProvider provider) =>
      logger = provider.GetRequiredService<ILogger<E>>();

   protected Task<IActionResult> ReadAsync<T>(Task<T> task) => TryAsync(() => task);

   protected async Task<IActionResult> WriteAsync(Task task) =>
      await TryAsync(async () => { await task; return true; });

   private async Task<IActionResult> TryAsync<T>(Func<Task<T>> task)
   {
      try
      {
         return Ok(await task());
      }
      catch (AuthenticationException ex)
      {
         logger.LogError(ex, ex.Message);
         return Unauthorized(ex);
      }
      catch (NullReferenceException ex)
      {
         logger.LogError(ex, ex.Message);
         return NotFound(ex);
      }
      catch (ArgumentException ex)
      {
         logger.LogError(ex, ex.Message);
         return BadRequest(ex);
      }
      catch (Exception ex)
      {
         logger.LogError(ex, "Erro inesperado");
         return StatusCode(500, "Erro inesperado");
      }
   }
}