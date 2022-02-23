using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using scrapy.net;

public class QuoteSpider : Spider<IResponse>
{
    private readonly ChromeDriver driver;
    private readonly ILogger<QuoteSpider> logger;

    public override string Name => "quotes";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/";

    public QuoteSpider(ILogger<QuoteSpider> logger) : base()
    {
        //var options = new ChromeOptions();
        //options.AddArguments("--test-type", "--start-maximized");
        //options.AddArguments("--test-type", "--ignore-certificate-errors");
        //driver = new ChromeDriver(".", options);

        driver = new ChromeDriver(".");
        logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Start Requests");
            var request = GetRequest<EmptyRequest>(request =>
            {
                request.Url = StartUrl;
                request.CallBack = () => LoginAsync(request, cancellationToken: cancellationToken);
            });
            return await request.ExecuteAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error!");
            throw;
        }
        finally
        {
            driver.Close();
        }
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"URL: {response.Url}");
        driver.Navigate().GoToUrl(response.Url);
        var quotes = driver.FindElements(By.CssSelector("div.quote"));

        foreach(var quote in quotes)
        {
            var item = new 
            {
                Text = quote.FindElement(By.CssSelector("span.text")).Text, 
                Author = quote.FindElement(By.CssSelector("small.author")).Text
            };
            response.Yield(item);
            logger.LogInformation($"Text: {item.Text} \nAuthor: {item.Author}\n");
        }

        var NextPage = driver.FindElements(By.CssSelector("li.next a")).FirstOrDefault();
        if (NextPage != null)
        {
            NextPage.Click();
            var url = driver.Url;
            response.Url = url;
            return await response.ExecuteAsync();
        }
        return await response.EndAsync();
    }

    private async Task<object?> LoginAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        //var wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(2));
        //wait.Until(driver => driver.FindElement(By.XPath("//a[@href='/beta/login']")));
        //var element = driver.FindElement(By.XPath("//a[@href='/beta/login']"));
        //wait.Until(driver => driver.FindElement(By.CssSelector("div.quote")));

        //element.SendKeys("username");

        response.CallBack = () => ParseAsync(response, cancellationToken: cancellationToken);
        return await response.ExecuteAsync();
    }
}