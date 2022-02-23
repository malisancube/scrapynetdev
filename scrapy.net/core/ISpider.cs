namespace scrapy.net;

public interface ISpider<IResponse>
{
    //public Task<IResponse> StartRequestsAsync(CancellationToken cancellationToken);
    public Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default);

}

