using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;
using MediatR;
using System.Security.Authentication;
using System.Security.Claims;


namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoHandler : AbstractHandler
   , IRequestHandler<SaldoDiarioQuery, decimal>
   , IRequestHandler<CreditarCommand, bool>
   , IRequestHandler<DebitarCommand, bool>
{
   private readonly IUnitOfWork _unitOfWork;

   public LancamentoHandler(IServiceProvider provider) : base(provider)
   {
      _unitOfWork = provider.GetRequiredService<IUnitOfWork>();
   }

   public async Task<bool> Handle(DebitarCommand command, CancellationToken cancel) =>
      await (await GetService()).Debitar(command.valor, command.Data);

   public async Task<bool> Handle(CreditarCommand command, CancellationToken cancel) =>
      await (await GetService()).Creditar(command.valor, command.Data);

   public async Task<decimal> Handle(SaldoDiarioQuery query, CancellationToken cancel)
   {
      if (!DateOnly.TryParse(query.data, out var data))
         throw new ArgumentException($"Data inválida em {query.data}");

      return await (await GetService()).SaldoDiario(data);
   }

   private async Task<LancamentoService> GetService()
   {
      var email = provider.GetRequiredService<IHttpContextAccessor>()
         .HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

      var usuario = await _unitOfWork.Usuarios.LoadAsync(x => x.Email == email);
      if (usuario is null) throw new UnauthorizedAccessException("Sem usuário logado");

      return new LancamentoService(provider, usuario.Id);
   }
}