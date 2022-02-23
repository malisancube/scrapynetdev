using Microsoft.Extensions.Logging;
using scrapy.net;

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
app.MapSpider("google");

await app.RunAsync();
