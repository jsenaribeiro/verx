using FluxoCaixa;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;

namespace FluxoCaixa.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ApiController<Domain.Usuarios.Usuario>
{
   public UsuarioController(IServiceProvider provider) : base(provider) { }

   [HttpPost("signup")]
   public Task<IActionResult> SignUp([FromBody] SignUpCommand command) =>
      TryAsync(() => mediator.Send(command));

   [HttpPost("signin")]
   public Task<IActionResult> SignIn([FromBody] SignInQuery query) =>
      TryAsync(() => mediator.Send(query));

   [Authorize]
   [HttpPost("signout")]
   new public Task<IActionResult> SignOut() =>
      TryAsync(() => mediator.Send(new SignOutCommand()));
}