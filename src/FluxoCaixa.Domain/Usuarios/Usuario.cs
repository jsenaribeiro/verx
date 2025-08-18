using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Usuarios;

public class Usuario : Entity
{
   public Usuario(string nome, string email, string senha)
   {
      this.Nome = nome;
      this.Email = email;
      this.Senha = senha;
   }

   public string Nome { get; private set; }

   public string Email { get; private set; }

   public string Senha { get; private set; }
}