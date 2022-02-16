using Microsoft.Extensions.Options;

namespace scrapy.net;

public static class ServiceCollectionExtensions
{
    public static IServiceProvider MapSpiders(this IServiceProvider serviceProvider)
    {
        return serviceProvider;
    }

    public static IServiceProvider MapSpider(this IServiceProvider serviceProvider, string name, Action<ScrapyApplicationOptions> options)
    {
        var app = serviceProvider.GetRequiredService<ScrapyApplication>();
        options.Invoke(app.ScrapyApplicationOptions);
        return serviceProvider;
    }

    public static void RegisterDefaultServices(this IServiceCollection services)
    {
        services.AddOptions<DefaultSettings>();
        services.AddSingleton<IScrapyEngine, DefaultEngine>();
        services.AddScoped<EmptyRequest>();
        services.AddScoped<HtmlRequest>();
        services.AddScoped<JsonRequest>();
    }

    public static void RegisterHttpServices(this IServiceCollection services, Func<IServiceProvider, IOptions<DefaultSettings>> implementationFactory)
    {
        services.AddSingleton(settings =>
        {
            var defaultSettings = settings.GetRequiredService<IOptions<DefaultSettings>>();
            return new ProxySettings(defaultSettings.Value.ProxiesFile);
        });
        services.AddScoped<IProxyService, ProxyService>();

        var httpClientBuilder = services.AddHttpClient("YYYY");

        httpClientBuilder.AddHttpMessageHandler(t =>
            {
                var proxyService = t.GetRequiredService<IProxyService>();
                return proxyService.GetHandler();
            });

        services.AddScoped<ICookieService, CookieService>();
        httpClientBuilder.AddHttpMessageHandler(t =>
        {
            var cookieService = t.GetRequiredService<ICookieService>();
            return cookieService.GetHandler();
        });
    }
}

