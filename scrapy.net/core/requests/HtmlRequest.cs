using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace scrapy.net;

public class HtmlRequest : BaseRequest
{
    private readonly HttpClient _httpClient;
    private readonly HtmlParser _parser;
    private IHtmlDocument _html;
    public ILogger<HtmlRequest> Logger { get; }
    public DefaultSettings Settings { get; }
    public Statistics Statistics { get; }

    public IHtmlDocument Html 
    { 
        get 
        {
            return _html;
        } 
    }

    public HtmlRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory, 
        IOptions<DefaultSettings> settings, 
        ILogger<HtmlRequest> logger) : base(serviceProvider, httpClientFactory, settings)
    {
        Settings = settings.Value;
        Logger = logger;

        _httpClient = httpClientFactory.CreateClient();
        _parser = new HtmlParser();

        AddHeaders(_httpClient, settings);
    }

    public override async Task<object?> ExecuteAsync([CallerMemberName] string callerName = "")
    {
        try
        {
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

    public override async Task<object?> EndAsync([CallerMemberName] string callerName = "")
    {
        return await Task.FromResult(new EndToken(Statistics.Instance));
    }

    private void AddHeaders(HttpClient httpClient, IOptions<DefaultSettings> settings)
    {
        foreach (var header in settings.Value.DefaultRequestHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public override void Yield(object item)
    {


        Statistics.Instance.Items++;
        //Settings.
    }


}
