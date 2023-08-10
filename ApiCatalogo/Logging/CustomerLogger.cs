namespace ApiCatalogo.Logging;

public class CustomerLogger : ILogger
{
    private readonly string loggerName;
    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string mensagem = $"{logLevel}: {eventId.Id} - {formatter(state, exception)} - {loggerName}";

        EscreverNoArquivo(mensagem);
    }

    private static void EscreverNoArquivo(string mensagem)
    {
        string caminhoArquivoLog = @"C:\Users\admin\source\repos\ApiCatalogo\ApiCatalogo\Logging\logArquivo.txt";

        using StreamWriter streamWriter = new(caminhoArquivoLog, true);
        try
        {
            streamWriter.WriteLine(mensagem);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            streamWriter.Close();
        }
    }
}
