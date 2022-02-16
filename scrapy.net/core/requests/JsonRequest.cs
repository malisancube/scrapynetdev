using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace scrapy.net;

public class JsonRequest : BaseRequest
{
    private readonly JsonSerializerOptions _options;
    private readonly HttpClient _httpClient;
    private JsonDocument _jsonDocument;

    public JsonDocument JsonDocument
    {
        get { return _jsonDocument; }
    }

    public ILogger<JsonRequest> Logger { get; }
    public Statistics Statistics { get; }

    public JsonRequest(IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        IOptions<DefaultSettings> settings,
        ILogger<JsonRequest> logger) : base(serviceProvider, httpClientFactory, settings)
    {
        _httpClient = httpClientFactory.CreateClient();
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        AddHeaders(_httpClient, settings);
        Logger = logger;
    }

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
        throw new NotImplementedException();
    }
}