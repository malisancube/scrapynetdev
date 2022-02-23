using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace scrapy.net;

public class ScrapyApplication
{
    public IServiceCollection Services { get; set; }

    public ScrapyApplication  Application { get; set; }

    public Dictionary<string, Type> Scrapers { get; set; } = new Dictionary<string, Type>();

    public IServiceProvider ServiceProvider { get; set; }

    public HostingEnvironment HostingEnvironment { get; set; }

    public ConcurrentBag<Spider<IResponse>> Spiders { get; set; }

    public ApplicationSettings Settings { get; set; }

    public IConfiguration Configuration { get; set; }

    public ILogger<ScrapyApplication> Logger { get; set; }

    public EventHandler<EngineStartedEventArgs> EngineStarted  { get; set; }

    public EventHandler<EngineStoppedEventArgs> EngineStopped  { get; set; }

    public static ScrapyApplication CreateBuilder(string[] args)
    {
        var app = new ScrapyApplication();
        app.Application = app;
        app.Services = new ServiceCollection();
        app.Spiders = new ConcurrentBag<Spider<IResponse>>();

        app.HostingEnvironment = new HostingEnvironment
        {
            ContentRootPath = Directory.GetCurrentDirectory()
        };

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

    public ScrapyApplication Build()
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
        //config.Configure(configBuilder =>
        //{
        //    config.Invoke(this, configBuilder);
        //});
        return this;
    }

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

     public IServiceProvider MapSpider(string name)
    {
        return MapSpider(name, options => { });
    }
 
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

    public IServiceProvider MapSpiders()
    {
        return MapSpiders(options => { });
    }

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
            Services.AddSingleton(typeof(SpiderPipelineItemBase), pipelineItem);
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
}