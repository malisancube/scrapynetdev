using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSpider("quotes");

await app.RunAsync(cancellationToken);