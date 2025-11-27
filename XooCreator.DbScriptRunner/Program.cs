using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.DbScriptRunner;

var switchMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["--connection"] = "ConnectionString",
    ["--schema"] = "Schema",
    ["--scripts-path"] = "ScriptsPath",
    ["--rollbacks-path"] = "RollbacksPath",
    ["--dry-run"] = "DryRun",
    ["--rollback"] = "RollbackTarget"
};

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables() // Support ConnectionStrings__Postgres and friends.
    .AddEnvironmentVariables("DB_RUNNER_")
    .AddCommandLine(args, switchMappings)
    .Build();

var options = new RunnerOptions();
configuration.Bind(options);
options.ConnectionString ??= configuration.GetConnectionString("Postgres");
options.ApplyDefaults();

if (string.IsNullOrWhiteSpace(options.ConnectionString))
{
    Console.Error.WriteLine("Connection string is required. Use --connection or set ConnectionStrings__Postgres / DB_RUNNER_CONNECTION.");
    return 1;
}

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Information)
        .AddSimpleConsole(opts =>
        {
            opts.TimestampFormat = "HH:mm:ss ";
            opts.SingleLine = true;
        });
});

var logger = loggerFactory.CreateLogger<ScriptRunnerService>();
var runner = new ScriptRunnerService(options, logger);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cts.Cancel();
    logger.LogWarning("Cancellation requested. Attempting graceful shutdown...");
};

try
{
    return await runner.RunAsync(cts.Token);
}
catch (OperationCanceledException)
{
    logger.LogWarning("Script execution cancelled.");
    return 2;
}
catch (Exception ex)
{
    logger.LogError(ex, "Database script runner failed.");
    return 1;
}
