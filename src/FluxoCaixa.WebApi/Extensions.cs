using System.Reflection;
using System.Text;
using FluxoCaixa.Domain.Lancamentos;
using FluxoCaixa.Infrastructure.Adapters;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public static class Extensions
{
   public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddAuthentication(options =>
      {
         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
         var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey não está no appsettings.");

         var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

         options.TokenValidationParameters = new TokenValidationParameters
         {
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false,
            IssuerSigningKey = signingKey,
            ValidateIssuerSigningKey = true,
         };
      });

      return services;
   }

   public static IServiceCollection AddMediatorCQRS(this IServiceCollection services)
   {
      var assembly = Assembly.GetAssembly(typeof(LancamentoHandler))!;

      services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

      services.AddScoped(typeof(IRequestHandler<,>), typeof(CommandHandlerAdapter<,>));
      services.AddScoped(typeof(IRequestHandler<,>), typeof(QueryHandlerAdapter<,>));

      return services;
   }

   public static IServiceCollection AddSqlServerContext(this IServiceCollection services)
   {
      services.AddDbContext<SqlServerContext>(opt => opt.UseInMemoryDatabase("db"));

      return services;
   }
}