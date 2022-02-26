using System.Net;
using System.Text.Json;

namespace scrapy.net;

public static class CookieManager
{
    public static CookieContainer LoadFromFile(string cookiesFile)
    {
        var cookies = LoadCookies(cookiesFile);

        var cookieContainer = new CookieContainer();
        foreach (var cookie in cookies)
        {
            cookieContainer.Add(cookie);
        }
        return cookieContainer;
    }

    private static IList<Cookie>? LoadCookies(string cookiesFile)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var json = File.ReadAllText(cookiesFile);
        var cookies = JsonSerializer.Deserialize<IList<Cookie>>(json, options);
        return cookies;
    }
}