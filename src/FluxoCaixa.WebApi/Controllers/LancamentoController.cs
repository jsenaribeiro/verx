using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Domain.Lancamentos;

namespace FluxoCaixa.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LancamentoController : ApiController<Lancamento>
{
   public LancamentoController(IServiceProvider provider) : base(provider) { }

   /// <summary>
   /// Consultando saldo diário do usuário logado
   /// </summary>
   /// <param name="data">yyyy-MM-dd</param>
   [HttpGet("saldo/{data}")]
   public Task<IActionResult> Get([FromQuery] string data) =>
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