using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace scrapy.net;

/// <summary>
/// Html request helper class 
/// </summary>
public class HtmlRequest : BaseRequest
{
    private readonly HttpClient _httpClient;

    private readonly HtmlParser _parser;

    private IHtmlDocument _html;
    public ILogger<HtmlRequest> Logger { get; }
    public Statistics Statistics { get; }

    /// <summary>
    /// Gett the Html result document
    /// </summary>
    public IHtmlDocument Html 
    { 
        get 
        {
            return _html;
        } 
    }

    public HtmlRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory, 
        ILogger<HtmlRequest> logger) : base(serviceProvider, httpClientFactory)
    {
        Logger = logger;

        _httpClient = httpClientFactory.CreateClient();
        _parser = new HtmlParser();
    }

    /// <summary>
    /// Invoke the Html request with parameters and follow the callback if necessary
    /// </summary>
    /// <param name="callerName"></param>
    /// <returns>The structured result object</returns>
    public override async Task<object?> ExecuteAsync([CallerMemberName] string callerName = "")
    {
        // TODO : https://github.com/dotnet/extensions/issues/521
        // https://stackoverflow.com/questions/63203660/httpclient-with-multiple-proxies-while-handling-socket-exhaustion-and-dns-recycl
        try
        {
            //AddHeaders(_httpClient, Settings);

            var caller = callerName;
            
            var message = await _httpClient.GetAsync(Url);
            Body = await message.Content.ReadAsStringAsync();

            Statistics.Instance.Pages++;

            // Parse the html document
            _html = await _parser.ParseDocumentAsync(Body);

            if (CallBack != null)
            {
                var callback = CallBack.Invoke();
                return await callback;
            }
        }
        catch (HttpRequestException exception)
        {
            Logger.LogError(exception, "There was an error executing the HTTP request.");
        }
        return this;
    }

    /// <summary>
    /// Submits the end of requests marker to indicate the end of items being scraped
    /// </summary>
    /// <param name="callerName"></param>
    /// <returns></returns>
    public override async Task<object?> EndAsync([CallerMemberName] string callerName = "")
    {
        return await Task.FromResult(new EndRequestMarker(Statistics.Instance));
    }

    private void AddHeaders(HttpClient httpClient, ApplicationSettings settings)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        var allTables = DictionaryUtils.Merge(new[] { settings.Headers, Headers });
        foreach (var header in allTables)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
        // Overwrite using the closer implementation settings

    }

    /// <summary>
    /// Send the item to the exporter and possible selected <see cref="OutputFile"/>
    /// </summary>
    /// <param name="item"></param>
    public override void Yield(object item)
    {
        // Required to execute pipeline tasks
        base.Yield(item);

        // TODO: Use indexer to write to stats
        Statistics.Instance.Items++;
        //Settings.

        var spider = ((Spider<IResponse>)this.Spider);
        if (spider.OutputFile != null)
        {
            spider.OutputFile.Write(item);
        }
    }
}
