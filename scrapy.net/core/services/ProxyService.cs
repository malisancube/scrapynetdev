using System.Net;

namespace scrapy.net;

public class ProxyService : IProxyService
{
    public string[] Proxies { get; set; }
    public ProxySettings Settings { get; }

    public ProxyService(ProxySettings settings)
    {
        Settings = settings;
        if (!string.IsNullOrEmpty(Settings.ProxiesFile))
            Proxies = File.ReadAllLines(Settings.ProxiesFile);
    }

    private (IWebProxy?, string Username, string Password) GetProxy(string proxyUrl)
    {
        var username = string.Empty;
        var password = string.Empty;

        if (!string.IsNullOrEmpty(proxyUrl))
        {
            var proxyUri = new Uri(proxyUrl);
            var proxy = new WebProxy(string.Format("{0}://{1}:{2}", proxyUri.Scheme, proxyUri.Host, proxyUri.Port));

            // Check if Uri has user authentication specified
            if (!string.IsNullOrEmpty(proxyUri.UserInfo))
            {
                var credentials = proxyUri.UserInfo.Split(':');
                username = credentials[0];
                password = credentials[1];
                proxy.Credentials = new NetworkCredential(username, password);
            }
            return (proxy, username, password);
        }
        return (null, username, password);
    }

    private HttpClientHandler GetProxyHandler(string proxyUrl)
    {
        // First create a proxy object
        var (proxy, username, password) = GetProxy(proxyUrl);

        // Now create a client handler which uses that proxy
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
        };

        // Omit this part if you don't need to authenticate with the web server:
        if (Settings.NeedsAuthentication)
        {
            httpClientHandler.PreAuthenticate = Settings.NeedsAuthentication;
            httpClientHandler.UseDefaultCredentials = Settings.UseDefaultCredentials;

            // *** These creds are given to the web server, not the proxy server ***
            httpClientHandler.Credentials = new NetworkCredential(username, password);
            httpClientHandler.UseProxy = Settings.UseProxy;
        }
        return httpClientHandler;
    }

    public HttpClientHandler GetHandler()
    {
        var random = new Random();
        var proxyIndex = random.Next(0, Proxies.Length);
        return GetProxyHandler(Proxies[proxyIndex]);
    }
}