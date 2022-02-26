namespace scrapy.net;

/// <summary>
/// The base class for all Pipeline classes which would be run during spider execution or completion
/// </summary>
public abstract class SpiderPipelineItemBase
{
    /// <summary>
    /// Pipeline response item
    /// </summary>
    public IResponse Response { get; }

    /// <summary>
    /// The method that will be executed when an item in yielded by a <see cref="BaseRequest"/>
    /// </summary>
    /// <param name="spider">The parent spider using this pipeline</param>
    /// <param name="item">The yielded item</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The response result<</returns>
    public virtual Task<IResponse> ProcessItemAsync(Spider<IResponse> spider, object? item, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Response);
    }

    /// <summary>
    /// The method that will be executed when the spider closes
    /// </summary>
    /// <param name="spider">The parent spider using this pipeline</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The response result</returns>
    public virtual Task<IResponse> CloseSpiderAsync(Spider<IResponse> spider, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Response);
    }
}