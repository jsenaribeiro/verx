using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;
using FluxoCaixa.WebApi.Requests;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace FluxoCaixa.WebApi.Handlers;

public class UsuarioHandler : AbstractHandler
   , IRequestHandler<SignUpCommand, bool>
   , IRequestHandler<SignInQuery, string>
{
   public UsuarioHandler(IServiceProvider provider) : base(provider) { }

   public Task<bool> Handle(SignUpCommand request, CancellationToken cancellationToken) =>
      new UsuarioService(provider).Cadastrar(request.nome, request.senha, request.email);

   public async Task<string> Handle(SignInQuery request, CancellationToken cancellationToken)
   {
      var (email, senha) = (request.email, request.senha);

      await new UsuarioService(provider).Autenticar(email, senha);

      var tokenHandler = new JwtSecurityTokenHandler();
      var configuration = provider.GetRequiredService<IConfiguration>();
      var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
      var symmetricKey = new SymmetricSecurityKey(key);
      var algorithm = SecurityAlgorithms.HmacSha256Signature;
      
      var claims = new ClaimsIdentity(new[]
      {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "User")
      });

      var descriptor = new SecurityTokenDescriptor
      {
         Subject = claims,
         Expires = DateTime.UtcNow.AddHours(1),
         Issuer = configuration["Jwt:Issuer"],
         Audience = configuration["Jwt:Audience"],
         SigningCredentials = new SigningCredentials(symmetricKey, algorithm)
      };

      return tokenHandler.WriteToken(tokenHandler.CreateToken(descriptor));
   }
}