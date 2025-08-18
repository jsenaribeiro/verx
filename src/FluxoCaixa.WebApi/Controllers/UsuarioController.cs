using FluxoCaixa.Domain;
using FluxoCaixa.Domain.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.WebApi.Controllers;

public class UsuarioController : ApiController<Usuario>
{
   private IUnitOfWork unitOfWork;

   public UsuarioController(IServiceProvider provider) : base(provider) =>
      unitOfWork = provider.GetRequiredService<IUnitOfWork>();

   [HttpPost("signup")]
   public async Task<IActionResult> SignUp(string email, string senha)
   {
      // if (_usuarios.Any(u => u.Email == email))
      //    return BadRequest("Usuário já existe.");

      // var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
      // _usuarios.Add((email, senhaHash));

      // return Ok("Usuário criado com sucesso.");
      await Task.CompletedTask;
      return Ok();
   }

   [HttpPost("signin")]
   public async Task<IActionResult> SignIn(string email, string senha)
   {
      // var usuario = _usuarios.FirstOrDefault(u => u.Email == email);
      // if (usuario == default || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
      //    return Unauthorized("Credenciais inválidas.");

      // var token = GerarJwtToken(email);
      // return Ok(new { Token = token });
      await Task.CompletedTask;
      return Ok();
   }

   [Authorize]
   [HttpPost("signout")]
   public async Task<IActionResult> SignOut()
   {
      // var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
      // _tokenBlacklist.Add(token);
      // return Ok("Logout realizado com sucesso.");
      await Task.CompletedTask;
      return Ok();
   }
}