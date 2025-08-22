using FluxoCaixa.Domain.Commons;
using FluxoCaixa.Domain.Usuarios;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.CircuitBreaker;
using System.Security.Authentication;
using System.Security.Claims;


namespace FluxoCaixa.Domain.Lancamentos;

public class LancamentoHandler : AbstractHandler
   , IRequestHandler<SaldoDiarioQuery, decimal>
   , IRequestHandler<CreditarCommand, bool>
   , IRequestHandler<DebitarCommand, bool>
{
   private readonly CacheFacade _cache;
   private readonly IUnitOfWork _unitOfWork;
   private readonly IConfiguration _configuration;
   private readonly ILogger<LancamentoHandler> _logger;

   public LancamentoHandler(IServiceProvider provider) : base(provider)
   {
      _logger = provider.GetRequiredService<ILogger<LancamentoHandler>>();
      _configuration = provider.GetRequiredService<IConfiguration>();
      _unitOfWork = provider.GetRequiredService<IUnitOfWork>();
      _cache = provider.GetRequiredService<CacheFacade>();
   }

   public async Task<bool> Handle(DebitarCommand command, CancellationToken cancel) =>
      await (await GetService()).Debitar(command.valor, command.Data);

   public async Task<bool> Handle(CreditarCommand command, CancellationToken cancel) =>
      await (await GetService()).Creditar(command.valor, command.Data);

   public async Task<decimal> Handle(SaldoDiarioQuery query, CancellationToken cancel)
   {
      if (!DateOnly.TryParse(query.data, out var data))
         throw new ArgumentException($"Data inválida em {query.data}");

      var saldoCacheKey = $"timeout:{data}";
      var saldoFallbackCacheKey = $"fallback";

      try
      {
         var timeout = _configuration["Cache:Timeout"] ?? "00:00:10";
         var fallbackTimeout = _configuration["Cache:FallbackTimeout"] ?? "01:00:00";

         if (_cache.TryGet<decimal>(saldoCacheKey, out var cachedSaldo)) return cachedSaldo;

         // escopo de circuit break para consulta do bd
         var usuarioId = await GetUsuarioIdLoginAsync();
         var service = new LancamentoService(provider, usuarioId);
         var saldo = await service.SaldoDiario(data);

         // fallback distributed caching (long time)
         await _cache.SetAsync(saldoFallbackCacheKey, saldo, fallbackTimeout);

         // response distributed caching (short time)
         return await _cache.LetAsync(saldoCacheKey, saldo, timeout);
      }
      catch (BrokenCircuitException)
      {
         throw;
      }
      catch (Exception ex)
      {
         var fallbackException = $"Falha ao tentar retornar o saldo em {nameof(SaldoDiarioQuery)}";

         _logger.LogInformation($"Fallback cache in {nameof(SaldoDiarioQuery)}: {ex.Message}");

         if (_cache.TryGet<decimal>(saldoFallbackCacheKey, out var saldo)) return saldo;

         else throw new ApplicationException(fallbackException);
      }
   }

   private async Task<Guid> GetUsuarioIdLoginAsync()
   {
      var email = provider.GetRequiredService<IHttpContextAccessor>()
         .HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

      var usuario = await _unitOfWork.Usuarios.LoadAsync(x => x.Email == email);
      if (usuario is null) throw new UnauthorizedAccessException("Sem usuário logado");

      else return usuario.Id;
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