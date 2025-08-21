using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace FluxoCaixa.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LancamentoController : ApiController<Lancamento>
{

   private readonly IDistributedCache _cache;

   public LancamentoController(IServiceProvider provider, IDistributedCache cache) : base(provider)
   {
      _cache = cache;
   }

   /// <summary>
   /// Consultando saldo diário do usuário logado
   /// </summary>
   /// <param name="data">yyyy-MM-dd</param>   

   [HttpGet("{data}")]
   public async Task<IActionResult> Get(string data)
   {
      var cacheKey = $"saldo:{data}";
      var cached = await _cache.GetStringAsync(cacheKey);
      if (cached != null)
         return Ok(decimal.Parse(cached));

      var result = await mediator.Send(new SaldoDiarioQuery(data));
      await _cache.SetStringAsync(cacheKey, result.ToString(), new DistributedCacheEntryOptions
      {
         AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
      });
      return Ok(result);
   }

   /// <summary>
   /// Realiza crédito na data atual com o usuário logado
   /// </summary>
   /// <param name="command">Valor do crédito</param>
   [HttpPost]
   public Task<IActionResult> Post([FromBody] CreditarCommand command) =>
      TryAsync(() => mediator.Send(command));

   /// <summary>
   /// Realiza débito na data atual com o usuário logado
   /// </summary>
   /// <param name="command">Valor do débito</param>
   [HttpDelete]
   public Task<IActionResult> Delete([FromBody] DebitarCommand command) =>
      TryAsync(() => mediator.Send(command));
}