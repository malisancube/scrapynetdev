using Microsoft.Extensions.Options;

namespace scrapy.net;

public static class FileServicesExtensions
{

    public static void RegisterStorageServices(this IServiceCollection services, Func<IServiceProvider, IOptions<ApplicationSettings>> implementationFactory)
    {
        services.AddSingleton(settings =>
        {
            var defaultSettings = settings.GetRequiredService<IOptions<ApplicationSettings>>();
            return new MultiThreadFileWriter(defaultSettings.Value.ProxiesFile);
        });
    }

}

