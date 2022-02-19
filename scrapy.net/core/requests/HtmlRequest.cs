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
    }

    public override async Task<object?> ExecuteAsync([CallerMemberName] string callerName = "")
    {
        // TODO : https://github.com/dotnet/extensions/issues/521
        // https://stackoverflow.com/questions/63203660/httpclient-with-multiple-proxies-while-handling-socket-exhaustion-and-dns-recycl
        try
        {
            AddHeaders(_httpClient, Settings);

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

    private void AddHeaders(HttpClient httpClient, DefaultSettings settings)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        var allTables = DictionaryExtensions.Merge(new[] { settings.DefaultRequestHeaders, Headers });
        foreach (var header in allTables)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
        // Overwrite using the closer implementation settings

    }

    public override void Yield(object item)
    {


        Statistics.Instance.Items++;
        //Settings.
    }


}


public static class DictionaryExtensions
{

    public static Dictionary<string, string> Merge(IEnumerable<Dictionary<string, string>> dictionaries)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = new Dictionary<string, string>(comparer);
        foreach (var dict in dictionaries)
            foreach (var x in dict)
                result[x.Key] = x.Value;
        return result;
    }

    // Works in C#3/VS2008:
    // Returns a new dictionary of this ... others merged leftward.
    // Keeps the type of 'this', which must be default-instantiable.
    // Example: 
    //   result = map.MergeLeft(other1, other2, ...)
    public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
        where T : IDictionary<K, V>, new()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        T newMap = new T();
        foreach (IDictionary<K, V> src in
            (new List<IDictionary<K, V>> { me }).Concat(others))
        {
            // ^-- echk. Not quite there type-system.
            foreach (KeyValuePair<K, V> p in src)
            {
                newMap[p.Key] = p.Value;
            }
        }
        return newMap;
    }

    public static Dictionary<Key, Value> MergeInPlace<Key, Value>(this Dictionary<Key, Value> left, Dictionary<Key, Value> right)
    {
        if (left == null)
        {
            throw new ArgumentNullException("Can't merge into a null dictionary");
        }
        else if (right == null)
        {
            return left;
        }

        foreach (var kvp in right)
        {
            if (!left.ContainsKey(kvp.Key))
            {
                left.Add(kvp.Key, kvp.Value);
            }
        }

        return left;
    }

}