using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSpider("coin");

await app.RunAsync();