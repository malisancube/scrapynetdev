using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSpider("quotes", spider =>
{
    spider.UseProxies = false;
    spider.DefaultOutputFileExtention = ".csv";
});
await app.RunAsync();
