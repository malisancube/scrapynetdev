using scrapy.net;
using Microsoft.Extensions.Logging;

public class QuoteSpider : Spider<IResponse>
{
    public override string Name => "quotes";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/";

    private readonly Dictionary<string, string> Headers = new() {
        ["accept"] = "*/*",
        ["user-agent"] = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36",
        ["accept-language"] = "en-US,en;q=0.9,la;q=0.8",
        ["accept-encoding"] = "gzip, deflate, br"
    };

    private readonly ILogger<QuoteSpider> logger;

    private const string CookiesFile = "./google_cookies.json";

    public QuoteSpider(ILogger<QuoteSpider> logger) : base()
    {
        this.logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        var htmlRequest = GetRequest<HtmlRequest>(request =>
        {
            request.Url = StartUrl;
            request.Headers = Headers;
            request.Cookies = CookieManager.LoadFromFile(CookiesFile);
            request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        });
        return await htmlRequest.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        var htmlResponse = (HtmlRequest)response;
        foreach(var quote in htmlResponse.Html.QuerySelectorAll("div.quote"))
        {
            var item = new 
            {
                Text = quote.QuerySelector("span.text")?.TextContent, 
                Author = quote.QuerySelector("small.author")?.TextContent
            };
            response.Yield(item);
            logger.LogInformation($"Text: {item.Text} \nAuthor: {item.Author}\n");
        }

        var NextPage = htmlResponse.Html.QuerySelector("li.next a")?.GetAttribute("href");
        if (NextPage != null)
        {
            var uri = new Uri(response.Url);
            response.Url = uri.GetLeftPart(UriPartial.Authority) + NextPage; 
            return await response.ExecuteAsync();
        }
        return await response.EndAsync();
    }
}