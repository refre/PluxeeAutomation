using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluxeeAutomation.PlayWright;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
    .CreateLogger();

Pluxee? pluxee = null;

try
{
    Log.Information("Démarrage de l'application PluxeeAutomation");

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var services = new ServiceCollection();

    services.Configure<PluxeeSettings>(configuration.GetSection("PluxeeSettings"));
    services.AddSingleton(Log.Logger);
    services.AddSingleton<Pluxee>();

    var serviceProvider = services.BuildServiceProvider();

    pluxee = serviceProvider.GetRequiredService<Pluxee>();

    await pluxee.InitializeAsync();
    await pluxee.NavigateToPluxeeAsync();
    await pluxee.LoginAsync();
    await pluxee.ClickAccesEntreprisesAsync();
    await pluxee.ClickCreateNewOrderAsync();
    await pluxee.ClickElectronicLunchPassAsync();
    await pluxee.FillOrderFormAsync();
    await pluxee.ClickOrderLineNextStepAsync();

    Log.Information("Processus terminé avec succès");
    Environment.Exit(0);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Une erreur critique est survenue. Le processus va s'arrêter");
    Environment.Exit(1);
}
finally
{
    if (pluxee != null)
    {
        await pluxee.CloseAsync();
    }
    await Log.CloseAndFlushAsync();
}
