using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace scrapy.net;

/// <summary>
/// Empty request used normally to execute out-of-process tasks (e.g Selenium)
/// </summary>
public class EmptyRequest : BaseRequest
{
    public ILogger<EmptyRequest> Logger { get; }
    public ApplicationSettings Settings { get; }

    public EmptyRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory, 
        ILogger<EmptyRequest> logger) : base(serviceProvider, httpClientFactory)
    {
        Logger = logger;
    }

    /// <summary>
    /// Submits the end of requests marker to indicate the end of items being scraped
    /// </summary>
    /// <param name="callerName"></param>
    /// <returns></returns>
    public async override Task<object?> EndAsync([CallerMemberName] string callerName = "")
    {
        return await Task.FromResult(this);
    }

    /// <summary>
    /// Invoke the empty request with parameters and follow the callback if necessary
    /// </summary>
    /// <param name="callerName"></param>
    /// <returns>The structured result object</returns>
    public async override Task<object?> ExecuteAsync([CallerMemberName] string callerName = "")
    {
        if (CallBack != null)
        {
            var callback = CallBack.Invoke();
            return await callback;
        }
        return await Task.FromResult(this);
    }

    public override void Yield(object item)
    {
        // No Op
    }
}