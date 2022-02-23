using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSpiders();

var cancellationToken = new CancellationToken();
await app.RunAsync(cancellationToken);
