using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);

var app = builder.Build();
app.MapSpider("coin");

var cancellationToken = new CancellationToken();
await app.RunAsync(cancellationToken);

Console.WriteLine("End");