using scrapy.net;
using Microsoft.Extensions.Logging;

public class QuoteSpider : Spider<IResponse>
{
    private readonly ILogger<QuoteSpider> logger;

    public override string Name => "quotes";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/";

    public QuoteSpider(ILogger<QuoteSpider> logger) : base()
    {
        this.logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Start Requests");
        HtmlRequest? request = GetRequest<HtmlRequest>();
        request.Url = StartUrl;
        request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        return await request.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Parsed");
        var htmlResponse = (HtmlRequest)response;

        foreach (var quote in htmlResponse.Html.QuerySelectorAll("div.quote"))
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



