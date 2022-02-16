using scrapy.net;
using Microsoft.Extensions.Logging;

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
app.MapSpiders();

var cancellationToken = new CancellationToken();
await app.RunAsync(cancellationToken);

Console.WriteLine("End");