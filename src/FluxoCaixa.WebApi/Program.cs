using FluxoCaixa.Domain;
using FluxoCaixa.Domain.Lancamentos;
using FluxoCaixa.Domain.Usuarios;
using FluxoCaixa.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMediatorCQRS();
builder.Services.AddSqlServerContext();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(x => x.AddConsole());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddJwtBearer(builder.Configuration);
builder.Services.AddApiDocumentation();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();