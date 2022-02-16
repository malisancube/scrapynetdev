using System.Net;
using System.Text.Json;

namespace scrapy.net;

public class CookieService : ICookieService
{
    public CookieService(DefaultSettings settings)
    {
        Settings = settings;
    }

    public DefaultSettings Settings { get; }

    private HttpClientHandler GetCookiesHandler(string cookiesFile)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var json = File.ReadAllText(cookiesFile);
        var cookies = JsonSerializer.Deserialize<IList<Cookie>>(json, options);

        var cookieContainer = new CookieContainer();
        foreach (var cookie in cookies)
        {
            cookieContainer.Add(cookie);
        }

        var handler = new HttpClientHandler() { CookieContainer = cookieContainer });
        return handler;
    }


    public HttpClientHandler GetHandler()
    {
        return GetCookiesHandler(Settings.CookiesFile);
    }

}