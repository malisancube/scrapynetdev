using scrapy.net;
using Microsoft.Extensions.Logging;

public class GoogleSpider : Spider<IResponse>
{
    private readonly ILogger<GoogleSpider> logger;

    public override string Name => "google";

    public override string StartUrl => "https://www.google.com/search?q=scraper";

    public GoogleSpider(ILogger<GoogleSpider> logger):base()
    {
        this.logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Start Requests");
        var request = GetRequest<HtmlRequest>();
        request.Url = StartUrl;
        request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);

        return await request.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Parsed");
        var htmlResponse = (HtmlRequest)response;

        var links = htmlResponse.Html.Anchors;
        return await response.EndAsync();
    }
}