namespace scrapy.net;

public interface ISpider<IResponse>
{
    public Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default);
}