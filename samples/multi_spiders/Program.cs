using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSpiders();
await app.RunAsync();
