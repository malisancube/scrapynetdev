namespace scrapy.net;

public abstract class SpiderPipelineItemBase
{
    public IResponse Response { get; }
    public virtual Task<IResponse> ProcessItemAsync(Spider<IResponse> spider, object? item, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Response);
    }

    public virtual Task<IResponse> CloseSpiderAsync(Spider<IResponse> spider, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Response);
    }
}