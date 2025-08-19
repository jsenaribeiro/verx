using FluxoCaixa.Domain.Usuarios;

namespace FluxoCaixa.UnitTests;

public class UsuarioDTO
{
   public string Nome { get; set; } = "";

   public string Senha { get; set; } = "";

   public string Email { get; set; } = "";

   public Usuario ToUsuario()
   {
      var nome = Nome.Replace("\"", "");
      var senha = Senha.Replace("\"", "");
      var email = Email.Replace("\"", "");

      return new Usuario(nome, email, senha);
   }
}