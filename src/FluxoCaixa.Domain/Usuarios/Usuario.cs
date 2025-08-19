using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Usuarios;

public class Usuario : Entity
{
   public Usuario() { }

   public Usuario(string nome, string email, string senha)
   {
      this.Nome = nome;
      this.Email = email;
      this.Senha = senha;
   }

   public string Nome { get; set; } = "";

   public string Email { get; set; } = "";

   public string Senha { get; set; } = "";
}