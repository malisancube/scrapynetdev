using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;

namespace scrapy.net;

public class ScrapyApplication
{
    public IServiceCollection Services { get; set; }

    public ScrapyApplication  Application { get; set; }

    public ScrapyApplicationOptions ScrapyApplicationOptions { get; set; } = new ScrapyApplicationOptions();

    public Dictionary<string, Type> Scrapers { get; set; } = new Dictionary<string, Type>();

    public IServiceProvider ServiceProvider { get; set; }

    public HostingEnvironment HostingEnvironment { get; set; }

    public ConcurrentBag<Spider<IResponse>> Spiders { get; set; }

    public IOptions<DefaultSettings> Settings { get; set; }

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

        //app.Services.AddLogging(logging =>
        //   {
        //       logging.AddConsole();
        //       logging.AddDebug();
        //       logging.SetMinimumLevel(LogLevel.Debug);
        //   });
        app.Services.AddSingleton<ScrapyApplication>();
        app.Services.AddSingleton(typeof(IServiceCollection), _ => app.Services);
        app.Services.RegisterDefaultServices();
        app.Services.RegisterHttpServices(services =>
        {
            var defaultSettings = services.GetRequiredService<IOptions<DefaultSettings>>();
            return defaultSettings;
        });
        return app;
    }

    public ScrapyApplication Build()
    {
        AddSpiders();
        ServiceProvider = Services.BuildServiceProvider();
        Logger = ServiceProvider.GetRequiredService<ILogger<ScrapyApplication>>();
        return Application;
    }

    public async Task RunAsync(CancellationToken token = default)
    {
        Task Initialize(Spider<IResponse> spider)
        {
            Logger.LogInformation($"Initializing : {spider.Name}...");

            // TODO: Add initialization code
            return Task.CompletedTask;
        }

        var spiders = ServiceProvider.GetServices<Spider<IResponse>>();
        foreach (var spider in spiders)
        {
            Spiders.Add(spider);
        }

        Logger.LogInformation($"Spiders count: {spiders.Count()}...");

        if (EngineStarted != null)
        {
            EngineStarted.Invoke(this, new EngineStartedEventArgs(this, spiders));
        }

        var results = new List<object?>();

        await Parallel.ForEachAsync(Spiders, async (spider, token) =>
        {
            var items = await spider.StartRequestsAsync(token);
            results.Add(items);
        });

        //var tasks = Spiders.Select(s => s.StartRequestsAsync(token)).ToArray();
        //var results = Task.WhenAll(tasks);
        //var objects = await results;

        foreach (var result in results)
        {
            if (result is EndToken endToken)
            {
                // Call pipeline stopped event

                // Show statistics
                //DisplayStatistics(task, endToken.Statistics);
            }
        }

        // TODO: What if there was an error
        if (EngineStopped != null)
        {
            EngineStopped.Invoke(this, new EngineStoppedEventArgs(this, spiders));
        }
    }

    private void DisplayStatistics(Statistics statistics)
    {
        Logger.LogInformation($"Page (s): {statistics.Pages}");
    }

    public IServiceProvider MapSpider(string name)
    {
        return MapSpider(name, options => { });
    }
 
    public IServiceProvider MapSpider(string name, Action<ScrapyApplicationOptions> options)
    {
        var spider = ServiceProvider
            .GetServices<Spider<IResponse>>()
            .FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (spider == null)
        {
            throw new Exception($"Spider with name '{nameof(name)}' was not found. Ensure the Name property has been set.");
        }

        spider.Application = this;
        Spiders.Add(spider);

        Settings = ServiceProvider.GetRequiredService<IOptions<DefaultSettings>>();
        HostingEnvironment.ApplicationName = Settings.Value.BotName;
        HostingEnvironment.EnvironmentName = Settings.Value.EnvironmentName;

        // TODO: apply spider specific settings
        options.Invoke(ScrapyApplicationOptions);
        Settings.Value.ProxiesFile = ScrapyApplicationOptions.ProxiesFile;

        return UseMiddleWare();
    }

    public IServiceProvider MapSpiders()
    {
        return MapSpiders(options => { });
    }

    public IServiceProvider MapSpiders(Action<ScrapyApplicationOptions> options)
    {
        var spiders = ServiceProvider
            .GetServices<Spider<IResponse>>();

        if (!spiders.Any())
        {
            throw new Exception($"There are no spiders in the project. Please create a Spider inheriting from 'Spider<IResponse>'.");
        }

        foreach (var spider in spiders)
        {
            spider.Application = this;
            Spiders.Add(spider);
        }

        Settings = ServiceProvider.GetRequiredService<IOptions<DefaultSettings>>();
        HostingEnvironment.ApplicationName = Settings.Value.BotName;
        HostingEnvironment.EnvironmentName = Settings.Value.EnvironmentName;

        // TODO: apply spider specific settings
        options.Invoke(ScrapyApplicationOptions);
        return UseMiddleWare();
    }

    private IServiceProvider UseMiddleWare()
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

    private IServiceProvider AddSpiders()
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
}



