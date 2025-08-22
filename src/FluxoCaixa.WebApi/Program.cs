
using FluxoCaixa.Domain;
using FluxoCaixa.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.GetConfiguration();
var logger = builder.GetLogging();
var services = builder.Services;

logger.LogInformation("Iniciando serviço...");

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
   services.AddDistributedCache(configuration);
   services.AddFallbacks(configuration, logger);
   services.AddThottling(configuration);
   services.AddCors(configuration);
   services.AddDocumentation();

   var app = builder.Build();

   if (app.Environment.IsDevelopment())
   {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI();
   }
   else
   {
      app.UseExceptionHandler("/Error");
      app.UseHsts();
      app.UseHttpsRedirection();
   }
   
   app.UseCors();
   app.UseRateLimiter(); 
   app.UseResponseCaching();
   app.UseAuthentication();
   app.UseAuthorization();
   app.UseRouting();
   app.MapControllers();
   app.MapHealthChecks("/health");

   app.Run();
}
catch (Exception ex)
{
   logger.LogError(ex, "Erro na inicialização");
   throw;
}
finally
{
   logger.LogError("Serviço encerrado.");
   NLog.LogManager.Shutdown();
}
