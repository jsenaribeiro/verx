using System.Security.Authentication;
using FluxoCaixa.Domain.Commons;

namespace FluxoCaixa.Domain.Usuarios;

public class UsuarioService
{
   private readonly IUnitOfWork _unitOfWork;

   public UsuarioService(IServiceProvider provider)
   {
      _unitOfWork = provider.GetService(typeof(IUnitOfWork)) as IUnitOfWork
         ?? throw new ArgumentNullException(nameof(IUnitOfWork));
   }

   public async Task<bool> Cadastrar(string nome, string senha, string email)
   {
      if (nome is null) throw new ArgumentNullException(nameof(nome));
      if (senha is null) throw new ArgumentNullException(nameof(senha));
      if (email is null) throw new ArgumentNullException(nameof(email));

      var usuario = new Usuario(nome, email, senha);

      var existeEmail = await _unitOfWork.Usuarios
         .ExistsAsync(x => x.Email == usuario.Email);

      if (existeEmail) throw new ArgumentException("E-mail já cadastrado");

      await _unitOfWork.Usuarios.SaveAsync(usuario);

      return true;
   }

   public async Task<Usuario> Autenticar(string email, string senha)
   {
      if (senha is null) throw new ArgumentNullException(nameof(senha));
      if (email is null) throw new ArgumentNullException(nameof(email));

      var usuario = await _unitOfWork.Usuarios
         .LoadAsync(x => x.Email == email && x.Senha == senha);

      var excecao = "Credenciais inválidas";

      if (usuario is null) throw new AuthenticationException(excecao);

      else return usuario;
   }
}