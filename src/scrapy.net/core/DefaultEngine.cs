namespace scrapy.net;

/// <summary>
/// The default engine to instantiate the downloaders
/// </summary>
public class DefaultEngine : IScrapyEngine
{
    public IServiceCollection Services { get; set; }

    public DefaultEngine(IServiceCollection services)
    {
        services.AddSingleton<IScrapyDownloader, DefaultDownloader>();
        Services = services;
    }
}

public interface IScrapyEngine
{
}