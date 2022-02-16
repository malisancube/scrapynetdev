using scrapy.net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

var builder = ScrapyApplication.CreateBuilder(args);

builder.ConfigureAppConfiguration((context, config) =>
{

}).ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    //logging.AddConfiguration(context.Configuration.GetSection("Logging"));
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});

var app = builder.Build();
app.MapSpider("quotes", options =>
{
    options.Headers = new Dictionary<string, string>()
    {
        { "Agent", "FireFox" }
    };
});

var cancellationToken = new CancellationToken();
await app.RunAsync(cancellationToken);


public static class ProxySettingsExtensions
{
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
          // Handle HttpRequestExceptions, 408 and 5xx status codes
          .HandleTransientHttpError()
          // Handle 404 not found
          .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
          // Handle 401 Unauthorized
          .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
          // What to do if any of the above erros occur:
          // Retry 3 times, each time wait 1,2 and 4 seconds before retrying.
          .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
