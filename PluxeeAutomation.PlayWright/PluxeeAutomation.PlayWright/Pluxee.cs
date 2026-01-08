using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Serilog;

namespace PluxeeAutomation.PlayWright;
public class Pluxee
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private readonly PluxeeSettings _settings;
    private readonly ILogger _logger;

    public Pluxee(IOptions<PluxeeSettings> settings, ILogger logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.Information("Début de l'initialisation du navigateur");
        try
        {
            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                ExecutablePath = _settings.BrowserExecutablePath
            });

            _page = await _browser.NewPageAsync();
            _logger.Information("Navigateur initialisé avec succès");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors de l'initialisation du navigateur");
            throw new Exception($"Erreur lors de l'initialisation du navigateur (InitializeAsync): {ex.Message}", ex);
        }
    }

    public async Task NavigateToPluxeeAsync()
    {
        _logger.Information("Début de la navigation vers Pluxee");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            await _page.GotoAsync(_settings.PluxeeUrl);
            _logger.Information("Navigation vers Pluxee réussie");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors de la navigation vers Pluxee");
            throw new Exception($"Erreur lors de la navigation vers Pluxee (NavigateToPluxeeAsync): {ex.Message}", ex);
        }
    }

    public async Task LoginAsync()
    {
        _logger.Information("Début de la connexion");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            await _page.FillAsync("#IDToken1", _settings.Username);
            await _page.FillAsync("#IDToken2", _settings.Password);
            await _page.ClickAsync("input[name='Login.Submit']");
            _logger.Information("Connexion réussie");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors de la connexion");
            throw new Exception($"Erreur lors de la connexion (LoginAsync): {ex.Message}", ex);
        }
    }

    public async Task ClickAccesEntreprisesAsync()
    {
        _logger.Information("Début du clic sur 'Accès pour les entreprises'");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.GetByText("Accès pour les entreprises").ClickAsync();
            _logger.Information("Clic sur 'Accès pour les entreprises' réussi");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors du clic sur 'Accès pour les entreprises'");
            throw new Exception($"Erreur lors du clic sur 'Accès pour les entreprises' (ClickAccesEntreprisesAsync): {ex.Message}", ex);
        }
    }

    public async Task ClickCreateNewOrderAsync()
    {
        _logger.Information("Début du clic sur 'Créer nouvelle commande'");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            //await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForSelectorAsync("#createNewOrder");
            await _page.ClickAsync("#createNewOrder");
            _logger.Information("Clic sur 'Créer nouvelle commande' réussi");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors du clic sur 'Créer nouvelle commande'");
            throw new Exception($"Erreur lors du clic sur 'Créer nouvelle commande' (ClickCreateNewOrderAsync): {ex.Message}", ex);
        }
    }

    public async Task ClickElectronicLunchPassAsync()
    {
        _logger.Information("Début du clic sur 'Chèques repas électroniques'");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            await _page.WaitForSelectorAsync("#createOrderModal_ELECTRONIC_LUNCH_PASS");
            await _page.ClickAsync("#createOrderModal_ELECTRONIC_LUNCH_PASS");
            _logger.Information("Clic sur 'Chèques repas électroniques' réussi");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors du clic sur 'Chèques repas électroniques'");
            throw new Exception($"Erreur lors du clic sur 'Chèques repas électroniques' (ClickElectronicLunchPassAsync): {ex.Message}", ex);
        }
    }

    public async Task FillOrderFormAsync()
    {
        _logger.Information("Début du remplissage du formulaire de commande");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            await _page.WaitForSelectorAsync("#overviewBeneficiariesOrdersTableForm");

            await _page.Locator("input[id$='orderline_units']").FillAsync("19");
            await _page.Locator("input[id$='orderline_faceValue']").FillAsync("10,00 €");

            var nextMonth = DateTime.Now.AddMonths(1);
            var firstDayOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            var formattedDate = firstDayOfNextMonth.ToString("dd-MM-yyyy");

            var dateInputLocator = _page.Locator("input[id$='orderline_validityStartDateInputDate']");
            if (await dateInputLocator.CountAsync() > 0)
            {
                await dateInputLocator.FillAsync(formattedDate);
            }
            else
            {
                var altDateInputLocator = _page.Locator("input[id*='orderline_validityStartDate'][type='text']");
                await altDateInputLocator.FillAsync(formattedDate);
            }

            await _page.ClickAsync("#overviewBeneficiariesOrdersTableForm\\:commandButtonNextStep");
            _logger.Information("Formulaire de commande rempli avec succès");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors du remplissage du formulaire de commande");
            throw new Exception($"Erreur lors du remplissage du formulaire de commande (FillOrderFormAsync): {ex.Message}", ex);
        }
    }

    public async Task ClickOrderLineNextStepAsync()
    {
        _logger.Information("Début du clic sur 'Étape suivante' du formulaire");
        try
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

            await _page.WaitForSelectorAsync("#orderLineForm\\:commandButtonNextStep");
            await _page.ClickAsync("#orderLineForm\\:commandButtonNextStep");
            _logger.Information("Clic sur 'Étape suivante' réussi");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Erreur lors du clic sur 'Étape suivante' du formulaire");
            throw new Exception($"Erreur lors du clic sur 'Étape suivante' du formulaire (ClickOrderLineNextStepAsync): {ex.Message}", ex);
        }
    }

    public async Task CloseAsync()
    {
        if (_browser != null)
            await _browser.CloseAsync();

        _playwright?.Dispose();
    }
}
