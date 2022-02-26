using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace scrapy.net;

/// <summary>
/// The scrapy application host
/// </summary>
public class ScrapyApplication : IAsyncDisposable
{
    /// <summary>
    /// Services used by the application
    /// </summary>
    public IServiceCollection Services { get; set; }

    public ScrapyApplication  Application { get; set; }

    /// <summary>
    /// The ServiceProvider to enable dependeency resolution 
    /// </summary>
    public IServiceProvider ServiceProvider { get; internal set; }

    /// <summary>
    /// Get and sets hosting environment
    /// </summary>
    public HostingEnvironment HostingEnvironment { get; set; }

    /// <summary>
    /// The set of <see cref="Spider<IResponse>"/> items that will be run during scraping
    /// </summary>
    public ConcurrentBag<Spider<IResponse>> Spiders { get; set; } = new ConcurrentBag<Spider<IResponse>>();

    /// <summary>
    /// Application settings that form the default for all spiders
    /// </summary>
    public ApplicationSettings Settings { get; set; }

    /// <summary>
    /// Configuration information received from <see cref="IConfiguration"/>
    /// </summary>
    public IConfigurationRoot Configuration { get; set; } 

    /// <summary>
    /// Sets and gets application logger
    /// </summary>
    public ILogger<ScrapyApplication> Logger { get; set; }

    public EventHandler<EngineStartedEventArgs> EngineStarted  { get; set; }

    public EventHandler<EngineStoppedEventArgs> EngineStopped  { get; set; }

    /// <summary>
    /// Configuration builder to enable application and spider setup
    /// </summary>
    private IConfigurationBuilder? _configurationBuilder { get; }

    private ScrapyApplication(string[]? args = null)
    {
        HostingEnvironment = new HostingEnvironment
        {
            ContentRootPath = Directory.GetCurrentDirectory(),
            EnvironmentName = "development",
        };

        _configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{HostingEnvironment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables(); 

        if (args != null && args.Any())
        {
            _configurationBuilder.AddCommandLine(args);
        }

        Configuration = _configurationBuilder.Build();
        Services = new ServiceCollection();
        Spiders = new ConcurrentBag<Spider<IResponse>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrapyApplication"/> class with preconfigured defaults.
    /// </summary>
    /// <param name="args">An array of params</param>
    /// <returns>ScrapyApplication instance</returns>
    public static ScrapyApplication CreateBuilder(string[]? args = null)
    {
        var app = new ScrapyApplication(args);
        app.Application = app;
        app.Services.AddSingleton<ScrapyApplication>();
        app.Services.AddSingleton(typeof(IServiceCollection), _ => app.Services);
        app.Services.RegisterDefaultServices();
        app.Services.RegisterHttpServices(services =>
        {
            var defaultSettings = services.GetRequiredService<IOptions<ApplicationSettings>>();
            return defaultSettings;
        });
        return app;
    }


    /// <summary>
    /// Build the application and initialize the registered services
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns>ScrapyApplication instance</returns>
    public ScrapyApplication Build(Assembly? assembly = null)
    {
        RegisterSpiders();
        ServiceProvider = Services.BuildServiceProvider();
        Logger = ServiceProvider.GetRequiredService<ILogger<ScrapyApplication>>();
        return Application;
    }

    public ScrapyApplication ConfigureLogging(Action<ScrapyApplication, ILoggingBuilder> logging)
    {
        Services.AddLogging(logbuilder =>
        {
            logging.Invoke(this, logbuilder);
        });
        return this;
    }

    public ScrapyApplication ConfigureAppConfiguration(Action<ScrapyApplication, IConfigurationBuilder> config)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        config.Invoke(this, _configurationBuilder);
#pragma warning restore CS8604 // Possible null reference argument.
        Configuration = _configurationBuilder.Build();
        return this;
    }

    /// <summary>
    /// Start the application.
    /// </summary>
    /// <param name="token"></param>
    /// <returns>
    /// A <see cref="Task"/> that represents the startup of the <see cref="ScrapyApplication"/>.
    /// Successful completion indicates the spiders have been run.
    /// </returns>
    public async Task RunAsync(CancellationToken token = default)
    {
        Logger.LogInformation($"Spiders count: {Spiders.Count()}...");
        if (EngineStarted != null)
        {
            EngineStarted.Invoke(this, new EngineStartedEventArgs(this, Spiders));
        }

        var results = new List<object?>();
        await Parallel.ForEachAsync(Spiders, async (spider, token) =>
        {
            var items = await spider.StartRequestsAsync(token);
            results.Add(items);
            spider.Close();
        });

        foreach (var result in results)
        {
            if (result is EndRequestMarker endRequestMarker)
            {
                // Call pipeline stopped event

                // Add result items to staistics
            }
        }

        // TODO: What if there was an error
        if (EngineStopped != null)
        {
            EngineStopped.Invoke(this, new EngineStoppedEventArgs(this, Spiders));
        }

        foreach(var result in results)
        {
            // Show statistics
            //DisplayStatistics(task, endToken.Statistics);
        }
    }

    /// <summary>
    /// Adds spider using name
    /// </summary>
    /// <param name="name">Name of the spider</param>
    /// <returns>Service provider</returns>
     public IServiceProvider MapSpider(string name)
    {
        return MapSpider(name, options => { });
    }
 
    /// <summary>
    /// Adds spiders using custom settings
    /// </summary>
    /// <param name="name">The name of the spider to be used</param>
    /// <param name="options">The delegate to apply spider settings</param>
    /// <returns>Service provider</returns>
    /// <exception cref="Exception"></exception>
    public IServiceProvider MapSpider(string name, Action<SpiderSettings> options)
    {
        var spider = ServiceProvider
            .GetServices<Spider<IResponse>>()
            .FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (spider == null)
        {
            throw new Exception($"Spider with name '{nameof(name)}' was not found. Ensure the Name property has been set.");
        }

        return MapSpidersCore(new[] { spider }, options);
    }

    /// <summary>
    /// Add spiders with default options
    /// </summary>
    /// <returns></returns>
    public IServiceProvider MapSpiders()
    {
        return MapSpiders(options => { });
    }

    /// <summary>
    /// Add spiders with custom settings
    /// </summary>
    /// <param name="options"></param>
    /// <returns>The service provider</returns>
    public IServiceProvider MapSpiders(Action<SpiderSettings> options)
    {
        var spiders = ServiceProvider
            .GetServices<Spider<IResponse>>();

        return MapSpidersCore(spiders, options);
    }

    private IServiceProvider MapSpidersCore(IEnumerable<Spider<IResponse>> spiders, Action<SpiderSettings> options)
    {
        if (!spiders.Any())
        {
            throw new Exception($"There are no spiders in the project. Please create a Spider inheriting from 'Spider<IResponse>'.");
        }

        foreach (var spider in spiders)
        {
            spider.Application = this;

            // Apply default settings
            var settings = ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
            var defaultSpiderSettings = ObjectUtils.DeepClone<ApplicationSettings, SpiderSettings>(settings.Value);
            spider.SpiderSettings = defaultSpiderSettings;

            // Overwrite them with specific spider settings if set
            options.Invoke(spider.SpiderSettings);
            spider.Initialize();
            Spiders.Add(spider);
        }
        CheckSpiderDuplicates();
        return RegisterPiperlines();
    }

    private void CheckSpiderDuplicates()
    {
        var duplicates = Spiders
            .GroupBy(s => s.Name)
            .Select(s => new { Name = s.Key, Count = s.Count() });

        if (duplicates.Any(s => s.Count > 1))
        {
            throw new Exception($"The Spider '{duplicates.First().Name}' has been registered more than once. Check 'app.MapSpider()'.");
        }
    }

    private IServiceProvider RegisterPiperlines()
    {
        var pipelinesItems = Assembly
            .GetEntryAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(SpiderPipelineItemBase)) && !t.IsAbstract)
            .Select(itemType => new SpiderPipelineDescriptior(itemType, itemType.GetCustomAttribute<PriorityAttribute>()?.Priority))
            .OrderBy(itemType => itemType.Priority);

        foreach (var pipelineItem in pipelinesItems)
        {
            Services.AddSingleton(pipelineItem.ItemType);
        }
        return ServiceProvider;
    }

    private IServiceProvider RegisterSpiders()
    {
        var spiders = Assembly
            .GetEntryAssembly()
            .GetTypes()
            .Where(t => typeof(Spider<IResponse>).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var spider in spiders)
        {
            Services.AddSingleton(typeof(Spider<IResponse>), spider);
        }
        return ServiceProvider;
    }

    private void DisplayStatistics(Statistics statistics)
    {
        Logger.LogInformation($"Page (s): {statistics.Pages}");
    }

    /// <summary>
    /// Disposes the application.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        return ((IAsyncDisposable)Application).DisposeAsync();
    }
}