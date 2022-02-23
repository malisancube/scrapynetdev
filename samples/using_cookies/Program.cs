using scrapy.net;
using Microsoft.Extensions.Logging;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();

app.MapSpider("quotes", spider =>
{
    spider.UseProxies = false;
});

await app.RunAsync();