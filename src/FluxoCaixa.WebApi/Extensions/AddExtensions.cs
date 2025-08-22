using System.Text;
using System.Reflection;
using FluxoCaixa.Domain.Lancamentos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Infrastructure;
using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;
using Microsoft.Extensions.DependencyInjection;

public static class AddExtensions
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

   public static IServiceCollection AddSqlServerContext(this IServiceCollection services, IConfiguration configuration, bool test = false)
   {
      var connection = configuration.GetConnectionString("DefaultConnection");

      if (test) services.AddDbContext<SqlServerContext>(opt => opt.UseInMemoryDatabase("db"));
      else services.AddDbContext<SqlServerContext>(opt => opt.UseSqlServer(connection));
      return services;
   }

   public static IServiceCollection AddDocumentation(this IServiceCollection services)
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

   public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration, bool teste = false)
   {
      services.AddScoped<CacheFacade>();
      services.AddResponseCaching();

      if (teste) services.AddDistributedMemoryCache();
      
      else services.AddStackExchangeRedisCache(options =>
      {
         options.Configuration = configuration["Cache:Redis"] ?? "localhost:6379";
         options.InstanceName = "caching:";
      });

      return services;
   }

   private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
   {
      return HttpPolicyExtensions
          .HandleTransientHttpError()
          .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
   }

   private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int exceptionsAllowedBeforeBreaking,
       int durationOfBreakInSeconds)
   {
      return HttpPolicyExtensions
          .HandleTransientHttpError()
          .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking, TimeSpan.FromSeconds(durationOfBreakInSeconds));
   }

   public static IServiceCollection AddFallbacks(this IServiceCollection services, IConfiguration configuration, ILogger logger)
   {
      var retry = configuration.GetValue<int>("Fallback:Retry");
      var limit = configuration.GetValue<int>("Fallback:Limit");
      var delay = configuration.GetValue<int>("Fallback:Delay");

      var progessiveTimeout = (int retryAttempt) =>
         TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

      var retryPolicy = HttpPolicyExtensions
          .HandleTransientHttpError()
          .WaitAndRetryAsync(retry, progessiveTimeout);

      var circuitBreakerPolicy = HttpPolicyExtensions
         .HandleTransientHttpError()
         .CircuitBreakerAsync(
               handledEventsAllowedBeforeBreaking: limit,
               durationOfBreak: TimeSpan.FromSeconds(delay),
               onBreak: (ex, breakDelay) => logger.LogInformation("Circuito aberto"),
               onReset: () => logger.LogInformation("Circuit break fechado (recovery)"),
               onHalfOpen: () => logger.LogInformation("Circuit break em teste (half-open)"));

      var policies = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

      services.AddHttpClient("*").AddPolicyHandler(policies);
      services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(policies);

      return services;
   }

   public static IServiceCollection AddThottling(this IServiceCollection services, IConfiguration configuration)
   {
      var windowLimitParam = configuration["Thottling:WindowLimit"] ?? "00:01:00";

      if (!TimeSpan.TryParse(windowLimitParam, out var windowLimit))
         windowLimit = TimeSpan.Parse("00:01:00");

      if (!int.TryParse(configuration["Thottling:PermitLimit"], out var permitLimit))
         permitLimit = 100;

      if (!int.TryParse(configuration["Thottling:QueuesLimit"], out var queuesLimit))
         queuesLimit = 2;

      services.AddRateLimiter(config => config
          .AddFixedWindowLimiter("default", o =>
          {
             o.PermitLimit = permitLimit;
             o.QueueLimit = queuesLimit;
             o.Window = windowLimit;
          }));

      return services;
   }

   public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
   {
      var origins = configuration["allowedOrigins"] ?? "*";

      if (origins == "*")
         services.AddCors(options => options
            .AddDefaultPolicy(policy => policy
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()));

      else services.AddCors(options => options
         .AddDefaultPolicy(policy => policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

      return services;
   }
}