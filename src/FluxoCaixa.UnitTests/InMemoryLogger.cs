using Microsoft.Extensions.Logging;

namespace FluxoCaixa.UnitTests;

public class InMemoryLogger<T> : ILogger<T>
{
   public static List<string> Logs = new();
   public static List<string> Messages = new();

   public IDisposable? BeginScope<TState>(TState state)
      where TState : notnull => null;

   public bool IsEnabled(LogLevel logLevel) => true;

   public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
   {
      if (formatter == null) throw new ArgumentNullException(nameof(formatter));

      var message = formatter(state, exception);

      if (exception != null) message += $" | Exception: {exception.Message}";

      Logs.Add($"{DateTime.Now:O} [{logLevel}] {typeof(T).Name}: {message}");
      Messages.Add(message);
   }
}