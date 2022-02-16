namespace scrapy.net;

public class DefaultSettings
{
    public string BotName = "scrapybot";

    public bool AutoThrottleEnabled = false;
    public bool AutoThrottleDebug = false;
    public double AutoThrottleMaxDelay = 60.0;
    public double AutoThrottleStartDelay = 5.0;
    public double AutoThrottleTargetConcurrency = 1.0;

    public int CloseSpiderTimeout = 0;
    public int CloseSpiderPageCount = 0;
    public int CloseSpiderItemCount = 0;
    public int CloseSpiderErrorCount = 0;

    public bool CompressionEnabled = false;

    public int ConcurrentItems = 100;

    public int ConcurrentRequests = 16;
    public int ConcurrentRequestsPerDomain = 8;
    public int ConcurrentRequestsPerIP = 8;

    public bool CookiesEnabled = false;
    public bool CookiesDebug = false;

    //public Type DefaultItemClass = typeof(CachingThreadedResolver);

    public Dictionary<string, string> DefaultRequestHeaders = new()
    {
        ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
        ["Accept-Language"] = "en"
    };

    public int DepthLimit = 0;
    public int DepthPriority = 0;
    public bool DepthStatsVerborse = false;

    public bool DNSCacheEnabled = false;
    public int DNSCacheSize = 10000;
    public int DNSTimeout = 60;
    //public Type DNSResolver = typeof(CachingThreadedResolver);

    public int DownloadDelay = 60;

    public Dictionary<string, Type> DownloadHandlerBase = new()
    {
        ["data"] = typeof(JsonItemExporter),
        ["file"] = typeof(FileDownloadHandler),
        ["http"] = typeof(HTTPDownloadHandler),
        ["https"] = typeof(HTTPDownloadHandler),
        ["s3"] = typeof(S3DownloadHandler),
        ["ftp"] = typeof(FTPDownloadHandler)
    };

    public int DownloadTimeout = 60;
    public int DownloadMaxSize = 1024 * 1024 * 1024;
    public int DownloadWarnSize = 32 * 1024 * 1024;
    public bool DownloadFailOnDataLoss = false;

    public Type Downloader = typeof(DefaultDownloader);
    public bool DownloaderStats = true;

    public Dictionary<Type, int> DownloaderMiddlewareBase = new()
    {
        [typeof(RobotsTxtMiddleware)] = 100
    };

    public Dictionary<string, Type> FeedExporterBase = new()
    {
        ["json"] = typeof(JsonItemExporter)
    };

    public string EnvironmentName = "Development";

    public string ProxiesFile = string.Empty;

    public string CookiesFile { get; internal set; }
}