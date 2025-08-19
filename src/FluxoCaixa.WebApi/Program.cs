using FluxoCaixa.Domain.Lancamentos;
using FluxoCaixa.Domain.Usuarios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMediatorCQRS();
builder.Services.AddSqlServerContext();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(x => x.AddConsole());
builder.Services.AddJwtBearer(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();