using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace scrapy.net;

/// <summary>
/// The base class for spiders that will scrap a site based on the sitemap.xml
/// </summary>
public class SitemapSpider : Spider<IResponse>
{
    public SitemapSpider(ILogger<Spider<IResponse>>? logger, IConfiguration? configuration, IHostEnvironment environment) : base(logger, configuration, environment)
    {
    }

    /// <inheritdoc />
    public override string Name => throw new NotImplementedException();

    /// <inheritdoc />
    public override string StartUrl => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Task<object?> StartRequestsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
