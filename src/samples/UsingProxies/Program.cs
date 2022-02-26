using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();

app.MapSpider("quotes", options =>
{
    options.Headers = new Dictionary<string, string>()
    {
        { "Agent", "FireFox" }
    };
    options.ProxiesFile = "./proxy_list.txt";
});

app.MapSpider("quotes");
await app.RunAsync();