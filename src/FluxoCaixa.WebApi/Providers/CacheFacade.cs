using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;

public class CacheFacade
{
   private readonly IDistributedCache _cache;
   private readonly ILogger<CacheFacade> _logger;

   public const string NOT_FOUND_KEY = "Chave de cacheamento '{0}' não encontrado";
   public const string INVALID_CACHED_TYPE = "Valor do cache '{0}' não tem o tipo esperado '{1}'";
   public const string INVALID_CACHED_VALUE = "Valor a ser cacheado não é nulo ou não pode ser stringified.";
   public const string INVALID_TIMEOUT_STRING = "O valor de timeout do cache deve ser no formato ##:##:##";

   public CacheFacade(IServiceProvider provider)
   {
      _cache = provider.GetRequiredService<IDistributedCache>();
      _logger = provider.GetRequiredService<ILogger<CacheFacade>>();
   }

   public async Task<T> GetAsync<T>(string key)
   {
      var ERRO_KEY = string.Format(NOT_FOUND_KEY, key);
      var ERRO_TYPE = string.Format(INVALID_CACHED_TYPE, key, typeof(T).Name);
         
      var cached = await _cache.GetStringAsync(key);
      if (cached is null) throw new ArgumentException(ERRO_KEY);

      try
      {
         _logger.LogWarning(string.Format(INVALID_CACHED_TYPE, key, typeof(T).Name));
         return (T)Convert.ChangeType(cached, typeof(T));
      }
      catch (Exception)
      {
         throw new ArgumentException(ERRO_TYPE);
      }
   }

   public bool TryGet<T>(string key, out T value)
   {
      try
      {
         value = GetAsync<T>(key).Result;
      }
      catch (Exception)
      {
         value = default;
         return false;
      }

      return true;
   }

   public async Task SetAsync<T>(string key, T value, string timeout)
   {
      if (!TimeSpan.TryParse(timeout, out var time))
         throw new ArgumentException(INVALID_TIMEOUT_STRING);

      else await SetAsync(key, value, time);
   }

   public async Task SetAsync<T>(string key, T value, TimeSpan timeout)
   {
      var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeout };

      if (value?.ToString() is not string content)
         throw new ArgumentException(INVALID_CACHED_VALUE);

      await _cache.SetStringAsync(key, content, options);
   }

   public async Task<T> LetAsync<T>(string key, T value, string timeout)
   {
      await SetAsync(key, value, timeout);
      return value;
   }

   public async Task<T> LetAsync<T>(string key, T value, TimeSpan timeout)
   {
      await SetAsync(key, value, timeout);
      return value;
   }
}