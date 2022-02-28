namespace scrapy.net;

public class ApplicationSettings
{
	public string BotName { get; set; } = "scrapybot";

	public bool CookiesEnabled { get; set; } = false;

	public bool CookiesDebug { get; set; } = false;

	public Dictionary<string, string> Headers { get; set; } = new()
	{
		["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
		["Accept-Language"] = "en"
	};

	public int DownloadDelay = 60;

	public bool DownloaderStats { get; set; } = true;

	public string EnvironmentName { get; set; } = "development";

	public bool UseProxies { get; set; } = false;

	public string ProxiesFile { get; set; } = "proxy_list.txt";

	public string CookiesFile { get; set; } = string.Empty;

	public string DefaultOutputFileExtention { get; set; } = ".jl";

	public ProxySettings ProxySettings { get; set; } = new ProxySettings();
}