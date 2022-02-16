# Scrapy.NET #

Scrapy.NET is a framework that gives you a similar API as Scrapy and gives you a  powerful ability to crawl websites and extract structured data from their pages. It can be used for a wide range of purposes ranging from data mining, monitoring and automated testing.

### Requirements

- .NET 6
- Works on Linux, Windows, MacOS, BSD 

### Getting Started 

Install the nuget package on your project as follows

```
Install-package Scrapy.NET
```

Use the `--version` option to specify a [preview version]() to install.

#### Basic Usage

You can then create your `Program.cs` as follows.

``` csharp
using scrapy.net;

var builder = ScrapyApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSpider("quotes");

await app.RunAsync();
```



The spider code would be 

``` csharp
using scrapy.net;

public class QuoteSpider : Spider<IResponse>
{
    public override string Name => "quotes";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/";

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        // Create request
        var httpRequest = GetRequest<HtmlRequest>(request =>
        {
            request.Url = StartUrl;
            request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        });
        return await httpRequest.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response)
    {
        var htmlResponse = (HtmlRequest)response;
        foreach (var quote in htmlResponse.Html.QuerySelectorAll("div.quote"))
        {
            var item = new 
            {
                Text = quote.QuerySelector("span.text")?.TextContent, 
                Author = quote.QuerySelector("small.author")?.TextContent
            };
            response.Yield(item);
            // Persist item into your preferred store
        }

        // Navigate to next page
        var NextPage = htmlResponse.Html.QuerySelector("li.next a")?.GetAttribute("href");
        if (NextPage != null)
        {
            var uri = new Uri(response.Url);
            response.Url = uri.GetLeftPart(UriPartial.Authority) + NextPage; 
            return await response.ExecuteAsync();
        }
        return await response.EndAsync();
    }
}

```



### Contributing

We welcome community pull requests for bug fixes, enhancements, and documentation. See [How to contribute](https://github.com/dotnet/efcore/blob/main/.github/CONTRIBUTING.md) for more information.

### Getting support

If you have a specific question about using these projects, we encourage you to [ask it on Stack Overflow](https://stackoverflow.com/questions/tagged/scrapy-net*?tab=Votes). If you encounter a bug or would like to request a feature, [submit an issue]().

## See also

- [Scrapy](https://github.com/scrapy/scrapy)

