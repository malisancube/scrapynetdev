using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace scrapy.net;

public abstract class Spider<TResponse> : ISpider<TResponse>  where TResponse : IResponse
{
    public abstract string Name { get; }

    public abstract string StartUrl { get; }

    public virtual List<string> AllowedDomains { get; }  = new List<string>();

    public virtual List<string> AllowedUrls { get; } = new List<string>();

    public ScrapyApplication Application { get; set; }

    public List<Type> Pipelines { get; set; } = new List<Type>();

    public ILogger<Spider<TResponse>> Logger { get; }

    public abstract Task<object?> StartRequestsAsync(CancellationToken cancellationToken);

    public abstract Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default);

    public Spider(ILogger<Spider<TResponse>>? logger = null, IConfiguration? configuration = null, IHostEnvironment environment = null)
    {
        Logger = logger;
    }

    public T GetRequest<T>() where T : BaseRequest
    {
        var request = Application.ServiceProvider.GetRequiredService<T>();
        request.Spider = this;
        return request;
    }

    public T GetRequest<T>(Action<T> action) where T : BaseRequest
    {
        var request = Application.ServiceProvider.GetRequiredService<T>();
        action.Invoke(request);
        request.Spider = this;
        return request;
    }

}

public interface ISpider<IResponse>
{
    //public Task<IResponse> StartRequestsAsync(CancellationToken cancellationToken);
    public Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default);

}