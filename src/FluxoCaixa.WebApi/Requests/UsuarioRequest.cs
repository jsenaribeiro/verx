using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;
using MediatR;

namespace FluxoCaixa.WebApi.Requests;

public partial record SignUpCommand(string nome, string email, string senha) : IRequest<bool>;

public partial record SignInQuery(string email, string senha) : IRequest<string>;

public partial record SignOutCommand() : IRequest<bool>;