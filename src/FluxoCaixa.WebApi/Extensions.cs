using System.Text;
using System.Reflection;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using NLog.Web;

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
         var secretKey = configuration["Jwt:Key"]
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

      return services;
   }

   public static IServiceCollection AddSqlServerContext(this IServiceCollection services, IConfiguration configuration)
   {
      var connection = configuration.GetConnectionString("DefaultConnection");

      services.AddDbContext<SqlServerContext>(opt => opt.UseSqlServer(connection));

      return services;
   }

   public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
   {
      var scheme = new OpenApiSecurityScheme
      {
         Name = "Authorization",
         In = ParameterLocation.Header,
         Type = SecuritySchemeType.Http,
         Scheme = "bearer",
         BearerFormat = "JWT",
         Description = "Digite: Bearer {seu token}"
      };

      var requirement = new OpenApiSecurityRequirement
      {
         {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                  Id = "Bearer",
                  Type = ReferenceType.SecurityScheme,
               }
            },
             Array.Empty<string>()
         }
      };

      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

      services.AddSwaggerGen(config =>
      {
         config.SwaggerDoc("v1", new OpenApiInfo { Title = "Lancamentos API", Version = "v1" });
         config.AddSecurityDefinition("Bearer", scheme);
         config.AddSecurityRequirement(requirement);
         config.IncludeXmlComments(xmlPath);
      });

      return services;
   }

   public static IServiceCollection AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
   {
      var connection = configuration.GetConnectionString("DefaultConnection");

      services.AddHealthChecks().AddSqlServer(connection!);

      return services;
   }

   public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
   {
      return services.AddApplicationInsightsTelemetry();
   }

   public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
   {
      services.AddResponseCaching();
      services.AddStackExchangeRedisCache(options =>
      {
         options.Configuration = configuration["Cache:Redis"] ?? "localhost:6379";
         options.InstanceName = "caching:";
      });

      return services;
   }

   public static NLog.Logger AddLogging(this WebApplicationBuilder builder)
   {
      builder.Logging.ClearProviders();
      builder.Logging.ClearProviders();
      builder.Logging.AddNLog();
      builder.Host.UseNLog();

      return NLog.LogManager.GetCurrentClassLogger();
   }
}