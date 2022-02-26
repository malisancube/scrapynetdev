using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace scrapy.net;

/// <summary>
/// The base class that represents all the spiders that can be instantiated and run
/// </summary>
/// <typeparam name="IResponse"></typeparam>
public abstract class Spider<IResponse> : ISpider<IResponse>, IDisposable 
{

    public abstract string Name { get; }
    /// <summary>
    /// The initial Url that the spider will start scraping
    /// </summary>
    public abstract string StartUrl { get; }

    /// <summary>
    /// Allowed domains
    /// </summary>
    public virtual List<string> AllowedDomains { get; }  = new List<string>();

    /// <summary>
    /// Allowed URLs
    /// </summary>
    public virtual List<string> AllowedUrls { get; } = new List<string>();

    /// <summary>
    /// The parent application instance
    /// </summary>
    public ScrapyApplication Application { get; set; }

    /// <summary>
    /// The pipelines that will be executed for each result item or when the spider completes scraping tasks
    /// </summary>
    public List<Type> Pipelines { get; set; } = new List<Type>();

    /// <summary>
    /// The Logger instance
    /// </summary>
    public ILogger<Spider<IResponse>> Logger { get; }

    /// <summary>
    /// Represents the instance of the output file where the spider output will be sent
    /// </summary>
    public OutputFile OutputFile { get; set; }

    /// <summary>
    /// Represents the spider settings
    /// </summary>
    public SpiderSettings SpiderSettings { get; set; }

    /// <summary>
    /// Runs the spider requests
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates a <see cref="BaseRequest"/> instance.
    /// </summary>
    /// <typeparam name="T">BaseRequest</typeparam>
    /// <returns>The <see cref="BaseRequest"/> descendant</returns>
    public T GetRequest<T>() where T : BaseRequest
    {
        var request = Application.ServiceProvider.GetRequiredService<T>();
        request.Spider = this;
        return request;
    }

    /// <summary>
    /// Creates a <see cref="BaseRequest"/> instance.
    /// </summary>
    /// <typeparam name="T">BaseRequest</typeparam>
    /// <param name="action">Assigns the initial parameters that correspond the <see cref="BaseRequest"/> instance.</param>
    /// <returns>The <see cref="BaseRequest"/> descendant</returns>
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
