using scrapy.net;
using Microsoft.Extensions.Logging;

[Priority(0)]
internal class UploadPipelineItem : SpiderPipelineItemBase
{
    public UploadPipelineItem(ILogger<UploadPipelineItem> logger)
    {
        Logger = logger;
    }

    public ILogger<UploadPipelineItem> Logger { get; }

    public override Task<IResponse> ProcessItemAsync(Spider<IResponse> spider, object? item, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Process Item");
        return base.ProcessItemAsync(spider, item, cancellationToken);
    }
}
