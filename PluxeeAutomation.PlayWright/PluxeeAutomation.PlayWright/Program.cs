using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluxeeAutomation.PlayWright;

Console.WriteLine("Launch Browser");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.Configure<PluxeeSettings>(configuration.GetSection("PluxeeSettings"));
services.AddSingleton<Pluxee>();

var serviceProvider = services.BuildServiceProvider();

var pluxee = serviceProvider.GetRequiredService<Pluxee>();
await pluxee.InitializeAsync();
await pluxee.NavigateToPluxeeAsync();
await pluxee.LoginAsync();
await pluxee.ClickAccesEntreprisesAsync();
await pluxee.ClickCreateNewOrderAsync();
await pluxee.ClickElectronicLunchPassAsync();
await pluxee.FillOrderFormAsync();
await pluxee.ClickOrderLineNextStepAsync();
await pluxee.CloseAsync();
