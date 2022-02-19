using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace scrapy.net;

public abstract class BaseRequest : IRequest, IResponse
{
    public string Proxy { get; set; }
    public string Url { get; set; }
    public string Method { get; set; }
    public string Body { get; set; }

    public Dictionary<string, string> Headers = new();
    public CookieContainer Cookies { get; set; }
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

///// <summary>
///// A wrapper class for <see cref="FlurlClient"/>, which solves socket exhaustion and DNS recycling.
///// </summary>
//public class FlurlClientManager
//{
//    /// <summary>
//    /// Static collection, which stores the clients that are going to be reused.
//    /// </summary>
//    private static readonly ConcurrentDictionary<string, IFlurlClient> _clients = new ConcurrentDictionary<string, IFlurlClient>();

//    /// <summary>
//    /// Gets the available clients.
//    /// </summary>
//    /// <returns></returns>
//    public ConcurrentDictionary<string, IFlurlClient> GetClients()
//        => _clients;

//    /// <summary>
//    /// Creates a new client or gets an existing one.
//    /// </summary>
//    /// <param name="clientName">The client name.</param>
//    /// <param name="proxy">The proxy URL.</param>
//    /// <returns>The <see cref="FlurlClient"/>.</returns>
//    public IFlurlClient CreateOrGetClient(string clientName, string proxy = null)
//    {
//        return _clients.AddOrUpdate(clientName, CreateClient(proxy), (_, client) =>
//        {
//            return client.IsDisposed ? CreateClient(proxy) : client;
//        });
//    }

//    /// <summary>
//    /// Disposes a client. This leaves a socket in TIME_WAIT state for 240 seconds but it's necessary in case a client has to be removed from the list.
//    /// </summary>
//    /// <param name="clientName">The client name.</param>
//    /// <returns>Returns true if the operation is successful.</returns>
//    public bool DeleteClient(string clientName)
//    {
//        var client = _clients[clientName];
//        client.Dispose();
//        return _clients.TryRemove(clientName, out _);
//    }

//    private IFlurlClient CreateClient(string proxy = null)
//    {
//        var handler = new SocketsHttpHandler()
//        {
//            Proxy = proxy != null ? new WebProxy(proxy, true) : null,
//            PooledConnectionLifetime = TimeSpan.FromMinutes(10)
//        };

//        var client = new HttpClient(handler);

//        return new FlurlClient(client);
//    }
//}


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