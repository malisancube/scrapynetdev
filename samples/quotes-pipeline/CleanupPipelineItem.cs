using scrapy.net;
using Microsoft.Extensions.Logging;

[Priority(1)]
internal class CleanupPipelineItem : SpiderPipelineItemBase
{
    public CleanupPipelineItem(ILogger<CleanupPipelineItem> logger)
    {
        Logger = logger;
    }

    public ILogger<CleanupPipelineItem> Logger { get; }

    public override Task<IResponse> CloseSpiderAsync(Spider<IResponse> spider, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Close spider");
        return base.CloseSpiderAsync(spider, cancellationToken);
    }
}
