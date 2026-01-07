using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace PluxeeAutomation.PlayWright;
public class Pluxee
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private readonly PluxeeSettings _settings;

    public Pluxee(IOptions<PluxeeSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            ExecutablePath = _settings.BrowserExecutablePath
        });

        _page = await _browser.NewPageAsync();
    }

    public async Task NavigateToPluxeeAsync()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        await _page.GotoAsync(_settings.PluxeeUrl);
    }

    public async Task LoginAsync()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        await _page.FillAsync("#IDToken1", _settings.Username);
        await _page.FillAsync("#IDToken2", _settings.Password);
        await _page.ClickAsync("input[name='Login.Submit']");
    }

    public async Task ClickAccesEntreprisesAsync()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.GetByText("Accès pour les entreprises").ClickAsync();
    }

    public async Task ClickCreateNewOrderAsync()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        //await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.WaitForSelectorAsync("#createNewOrder");
        await _page.ClickAsync("#createNewOrder");
    }

    public async Task ClickElectronicLunchPassAsync()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        await _page.WaitForSelectorAsync("#createOrderModal_ELECTRONIC_LUNCH_PASS");
        await _page.ClickAsync("#createOrderModal_ELECTRONIC_LUNCH_PASS");
    }

    public async Task FillOrderFormAsync()
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
    }

    public async Task ClickOrderLineNextStepAsync()
    {
        if (_page == null)
            throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");

        await _page.WaitForSelectorAsync("#orderLineForm\\:commandButtonNextStep");
        await _page.ClickAsync("#orderLineForm\\:commandButtonNextStep");
    }

    public async Task CloseAsync()
    {
        if (_browser != null)
            await _browser.CloseAsync();

        _playwright?.Dispose();
    }
}
