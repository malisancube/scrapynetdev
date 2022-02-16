using scrapy.net;
using Microsoft.Extensions.Logging;

public class CoinSpider : Spider<IResponse>
{
    private readonly ILogger<CoinSpider> logger;

    public override string Name => "coin";

    public override string StartUrl => "https://s2.coinmarketcap.com/generated/search/quick_search.json";

    public CoinSpider(ILogger<CoinSpider> logger) : base()
    {
        this.logger = logger;
    }

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Start Requests");
        var request = GetRequest<JsonRequest>();
        request.Url = StartUrl;
        request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        return await request.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Parsed");
        var jsonResponse = (JsonRequest)response;

        return await response.EndAsync();
    }

}



