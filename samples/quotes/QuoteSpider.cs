using scrapy.net;

public class QuoteSpider : Spider<IResponse>
{
    public override string Name => "quotes";

    public override string StartUrl => "http://quotes.toscrape.com/tag/humor/";

    public override async Task<object?> StartRequestsAsync(CancellationToken cancellationToken = default)
    {
        // Create reguest
        var httpRequest = GetRequest<HtmlRequest>(request =>
        {
            request.Url = StartUrl;
            request.CallBack = () => ParseAsync(request, cancellationToken: cancellationToken);
        });
        return await httpRequest.ExecuteAsync();
    }

    public override async Task<object?> ParseAsync(BaseRequest response,
        CancellationToken cancellationToken = default)
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