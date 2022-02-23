using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace scrapy.net;

public class EmptyRequest : BaseRequest
{
    public ILogger<EmptyRequest> Logger { get; }
    public ApplicationSettings Settings { get; }

    public EmptyRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory, 
        IOptions<ApplicationSettings> settings, 
        ILogger<EmptyRequest> logger) : base(serviceProvider, httpClientFactory, settings)
    {
        Settings = settings.Value;
        Logger = logger;
    }

    public async override Task<object?> EndAsync([CallerMemberName] string callerName = "")
    {
        return await Task.FromResult(this);
    }

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