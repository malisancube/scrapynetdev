using Microsoft.Extensions.DependencyInjection;

namespace scrapy.net;

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