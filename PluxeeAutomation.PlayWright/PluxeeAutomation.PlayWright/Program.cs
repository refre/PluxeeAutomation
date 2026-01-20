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
DateTime? customDate = null;

try
{
    Log.Information("Démarrage de l'application PluxeeAutomation");

    // Handle command-line parameters
    if (args.Length > 0)
    {
        if (int.TryParse(args[0], out int parameter))
        {
            switch (parameter)
            {
                case 0:
                    Log.Information("Mode automatique activé (paramètre 0)");
                    break;
                case 1:
                    Log.Information("Mode avec date personnalisée (paramètre 1)");
                    Console.WriteLine("Entrez une date (format jj/MM/aaaa) :");
                    string? dateInput = Console.ReadLine();

                    if (!string.IsNullOrEmpty(dateInput))
                    {
                        if (DateTime.TryParseExact(dateInput, "dd/MM/yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                        {
                            customDate = parsedDate;
                            Log.Information($"Date personnalisée définie : {customDate:dd/MM/yyyy}");
                        }
                        else
                        {
                            Log.Warning("Format de date invalide. Utilisation de la date par défaut.");
                        }
                    }
                    break;
                default:
                    Log.Warning($"Paramètre non reconnu : {parameter}. Mode automatique par défaut.");
                    break;
            }
        }
        else
        {
            Log.Warning("Paramètre invalide. Mode automatique par défaut.");
        }
    }
    else
    {
        Log.Information("Aucun paramètre fourni. Mode automatique par défaut.");
    }

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var services = new ServiceCollection();

    services.Configure<PluxeeSettings>(options =>
    {
        configuration.GetSection("PluxeeSettings").Bind(options);
        options.CustomDate = customDate;
    });
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
