namespace scrapy.net;

public class ProxySettings
{
	public bool NeedsAuthentication { get; set; } = false;
	public bool PreAuthenticate { get; set; } = true;
	public bool UseDefaultCredentials { get; set; } = false;
	public bool UseProxy { get; }
	public string ProxiesFile { get; }
	public bool RandomProxy { get; set; }

	public ProxySettings() : this("")
	{
	}

	public ProxySettings(string proxiesFile)
	{
		ProxiesFile = proxiesFile;
		UseProxy = !string.IsNullOrEmpty(ProxiesFile);
	}
}

