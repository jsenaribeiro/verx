namespace FluxoCaixa.WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

public abstract class ApiController<E> : ControllerBase where E : class
{
   protected readonly ILogger<E> logger;

   protected readonly IMediator mediator;

   public ApiController(IServiceProvider provider)
   {
      logger = provider.GetRequiredService<ILogger<E>>();
      mediator = provider.GetRequiredService<IMediator>();
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
         return Unauthorized(ex);
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