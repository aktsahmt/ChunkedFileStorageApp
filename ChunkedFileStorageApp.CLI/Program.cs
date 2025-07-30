using ChunkedFileStorageApp.Application.Services;
using ChunkedFileStorageApp.Core.Configurations;
using ChunkedFileStorageApp.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var configuration = BuildConfiguration();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Uygulama başlatılıyor...");

    var services = ConfigureServices(configuration);
    var serviceProvider = services.BuildServiceProvider();

    var app = new AppRunner(serviceProvider);
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılırken fatal bir hata oluştu.");
}
finally
{
    Log.CloseAndFlush();
}

IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
}

IServiceCollection ConfigureServices(IConfiguration configuration)
{
    var services = new ServiceCollection();

    services.AddLogging(loggingBuilder =>
        loggingBuilder.ClearProviders()
                      .AddSerilog(dispose: true));

    // Config Settings
    services.Configure<ChunkSettings>(configuration.GetSection("ChunkSettings"));

    // DI Extensions
    services.AddInfrastructure();

    return services;
}