using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;
using FluxoCaixa.WebApi.Requests;
using MediatR;

namespace FluxoCaixa.WebApi.Handlers;

public class UsuarioHandler : AbstractHandler
   , IRequestHandler<SignUpCommand, bool>
   , IRequestHandler<SignInQuery, Usuario>
{
   public UsuarioHandler(IServiceProvider provider) : base(provider) { }

   public Task<bool> Handle(SignUpCommand request, CancellationToken cancellationToken) =>
      new UsuarioService(provider).Cadastrar(request.nome, request.senha, request.email);

   public Task<Usuario> Handle(SignInQuery request, CancellationToken cancellationToken) =>
      new UsuarioService(provider).Autenticar(request.email, request.senha);
}