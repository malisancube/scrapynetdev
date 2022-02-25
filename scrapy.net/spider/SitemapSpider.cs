using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace scrapy.net;

public class SitemapSpider : Spider<IResponse>
{
    public SitemapSpider(ILogger<Spider<IResponse>>? logger, IConfiguration? configuration, IHostEnvironment environment) : base(logger, configuration, environment)
    {
    }

    public override string Name => throw new NotImplementedException();

    public override string StartUrl => throw new NotImplementedException();

    public override Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<object?> StartRequestsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
