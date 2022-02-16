using scrapy.net;
using Microsoft.Extensions.Logging;

public class QuotePage2Spider : Spider<IResponse>
{
    private readonly ILogger<QuotePage2Spider> logger;

    public override string Name => "quotes2";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/page/2/";

    public QuotePage2Spider(ILogger<QuotePage2Spider> logger) : base()
    {
        this.logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"Start Requests: {nameof(QuotePage2Spider)}");
        var request = GetRequest<HtmlRequest>(request =>
        {
            request.Url = StartUrl;
            request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        });
        return await request.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Parsed: {nameof(QuotePage2Spider)}");
        var htmlResponse = (HtmlRequest)response;

        foreach(var quote in htmlResponse.Html.QuerySelectorAll("div.quote"))
        {
            var item = new 
            {
                Text = quote.QuerySelector("span.text")?.TextContent, 
                Author = quote.QuerySelector("small.author")?.TextContent
            };
            response.Yield(item);
            logger.LogInformation($"\nSpider: {nameof(QuotePage2Spider)}\nText: {item.Text} \nAuthor: {item.Author}\n");
        }
        return await response.EndAsync();
    }

}



