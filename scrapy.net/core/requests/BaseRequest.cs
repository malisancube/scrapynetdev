using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace scrapy.net;

public abstract class BaseRequest : IRequest, IResponse
{
    public string Proxy { get; set; }
    public string Url { get; set; }
    public string Method { get; set; }
    public string Body { get; set; }
    public PropertyBag Headers { get; set; }
    public string Cookies { get; set; }
    public string ContentType { get; set; }
    public object? Args { get; set; }
    public Func<Task<object?>> CallBack { get; set; }
    public object Spider { get; set; }
    public IServiceProvider ServiceProvider { get; }

    private readonly IHttpClientFactory _httpClientFactory;

    private DefaultSettings Settings { get; }

    public BaseRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory, 
        IOptions<DefaultSettings> settings)
    {
        ServiceProvider = serviceProvider;
        Settings = settings.Value;
        _httpClientFactory = httpClientFactory;
    }

    public abstract Task<object?> ExecuteAsync([CallerMemberName] string callerName = "");

    public abstract Task<object?> EndAsync([CallerMemberName] string callerName = "");

    public virtual void Yield(object item)
    {
        // TODO : Cache this
        var spider = (Spider<IResponse>)Spider;
        var pipeline = spider
            .Pipelines
            .Select(itemType => new SpiderPipelineDescriptior(itemType, itemType.GetCustomAttribute<PriorityAttribute>()?.Priority))
            .OrderBy(itemType => itemType.Priority);

        foreach (var type in pipeline)
        {
            if (ServiceProvider.GetService(type.ItemType) is SpiderPipelineItemBase pipelineItem)
            {
                pipelineItem.ProcessItemAsync(spider, item, default);
            }
        }
    }
}


public abstract class BaseResponse : BaseRequest
{
    protected BaseResponse(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory, 
        IOptions<DefaultSettings> settings) : base(serviceProvider, httpClientFactory, settings)
    {
    }
}

//
// Finally, create the HTTP client object
//var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);