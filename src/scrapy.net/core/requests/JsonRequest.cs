using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace scrapy.net;

/// <summary>
/// Json request helper class 
/// </summary>
public class JsonRequest : BaseRequest
{
    private readonly JsonSerializerOptions _options;
    private readonly HttpClient _httpClient;
    private JsonDocument _jsonDocument;

    public JsonDocument JsonDocument
    {
        get { return _jsonDocument; }
    }

    /// <summary>
    /// Request logger
    /// </summary>
    public ILogger<JsonRequest> Logger { get; }

    public Statistics Statistics { get; }

    public JsonRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        ILogger<JsonRequest> logger) : base(serviceProvider, httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        Logger = logger;
    }

    /// <summary>
    /// Invoke the Json request with parameters and follow the callback if necessary
    /// </summary>
    /// <param name="callerName"></param>
    /// <returns>The structured result object</returns>
    public override async Task<object?> ExecuteAsync([CallerMemberName] string callerName = "")
    {
        try
        {
            var caller = callerName;

            var message = await _httpClient.GetStringAsync(Url);
            Body = message?.ToString() ?? string.Empty;
            _jsonDocument = JsonDocument.Parse(Body);
            Statistics.Instance.Pages++;

            // Parse the html document

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

    private void AddHeaders(HttpClient httpClient, IOptions<ApplicationSettings> settings)
    {
        foreach (var header in settings.Value.Headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public override void Yield(object item)
    {
        throw new NotImplementedException();
    }
}