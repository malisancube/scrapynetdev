using scrapy.net;
using Microsoft.Extensions.Logging;

public class QuotePage1Spider : Spider<IResponse>
{
    private readonly ILogger<QuotePage1Spider> logger;

    public override string Name => "quotes1";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/page/1/";

    public QuotePage1Spider(ILogger<QuotePage1Spider> logger) : base()
    {
        this.logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"Start Requests: {nameof(QuotePage1Spider)}");
        var request = GetRequest<HtmlRequest>();
        request.Url = StartUrl;
        request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        return await request.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Parsed: {nameof(QuotePage1Spider)}");
        var htmlResponse = (HtmlRequest)response;

        foreach(var quote in htmlResponse.Html.QuerySelectorAll("div.quote"))
        {
            var item = new 
            {
                Text = quote.QuerySelector("span.text")?.TextContent, 
                Author = quote.QuerySelector("small.author")?.TextContent
            };
            response.Yield(item);
            logger.LogInformation($"{nameof(QuotePage2Spider)}\nText: {item.Text} \nAuthor: {item.Author}\n");
        }

        return await response.EndAsync();
    }

}



