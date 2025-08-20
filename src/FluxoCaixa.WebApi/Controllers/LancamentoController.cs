using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.AspNetCore.Authorization;

namespace FluxoCaixa.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LancamentoController : ApiController<Lancamento>
{
   public LancamentoController(IServiceProvider provider) : base(provider) { }

   /// <summary>
   /// Consultando saldo diário do usuário logado
   /// </summary>
   /// <param name="data">yyyy-MM-dd</param>   
   [HttpGet("{data}")]
   public Task<IActionResult> Get(string data) =>
      TryAsync(() => mediator.Send(new SaldoDiarioQuery(data)));

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