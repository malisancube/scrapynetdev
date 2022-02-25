using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace scrapy.net;

public abstract class Spider<IResponse> : ISpider<IResponse>, IDisposable 
{
    public SpiderSettings SpiderSettings { get; set; }

    public abstract string Name { get; }

    public abstract string StartUrl { get; }

    public virtual List<string> AllowedDomains { get; }  = new List<string>();

    public virtual List<string> AllowedUrls { get; } = new List<string>();

    public ScrapyApplication Application { get; set; }

    public List<Type> Pipelines { get; set; } = new List<Type>();

    public ILogger<Spider<IResponse>> Logger { get; }

    public OutputFile OutputFile { get; set; }

    public abstract Task<object?> StartRequestsAsync(CancellationToken cancellationToken);

    public abstract Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default);

    public Spider(ILogger<Spider<IResponse>>? logger = null,  
        IConfiguration? configuration = null, 
        IHostEnvironment environment = null,
        IServiceProvider serviceProvider = null)
    {
        Logger = logger;
        
        var sep = serviceProvider;
        //InitializeSpider();
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
        InitializeRequest(ref request);

        // Overwrite the some of the defaults by assigning newer settings 
        action.Invoke(request);
        return request;
    }

    private void InitializeRequest<T>(ref T request) where T : BaseRequest
    {
        request.Spider = this;

        // Setup initial request defaults
        if (SpiderSettings.Headers.Any())
        {
            request.Headers = SpiderSettings.Headers;
        }
        request.Method = HttpConstants.HttpGet;
    }

    internal void Initialize()
    {
        if (SpiderSettings.OutputSettings.OutputType == OutputType.File)
        {
            SpiderSettings.OutputSettings.OutputFileName = Name + SpiderSettings.DefaultOutputFileExtention;
            OutputFile = new OutputFile(SpiderSettings.OutputSettings.OutputFileName);
        }
    }

    public void Close()
    {
        OutputFile?.Dispose();
    }

    public void Dispose()
    {
        // TODO: // Cleanup
    }
}
