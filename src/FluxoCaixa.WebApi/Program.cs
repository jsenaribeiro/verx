
using FluxoCaixa.Domain;
using FluxoCaixa.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var logger = builder.AddLogging();
var services = builder.Services;
var configuration = builder.Configuration;

try
{
   services.AddSwaggerGen();
   services.AddControllers();
   services.AddMediatorCQRS();
   services.AddHttpContextAccessor();
   services.AddEndpointsApiExplorer();
   services.AddScoped<IUnitOfWork, UnitOfWork>();
   services.AddJwtBearer(configuration);
   services.AddSqlServerContext(configuration);
   services.AddHealthCheck(configuration);
   services.AddTelemetry(configuration);
   services.AddDistributedCache(configuration);
   services.AddApiDocumentation();

   var app = builder.Build();

   app.UseSwagger();
   app.UseSwaggerUI();
   app.UseHttpsRedirection();
   app.UseResponseCaching();
   app.UseAuthentication();
   app.UseAuthorization();
   app.MapControllers();

   app.MapHealthChecks("/health");

   app.Run();
}
catch (Exception ex)
{
   logger.Error(ex, "Erro na inicialização");
   throw;
}
finally
{
   NLog.LogManager.Shutdown();
}
