namespace scrapy.net;

public class ScrapyApplicationOptions
{
    public Dictionary<string, string> Headers { get; set; }
    public string Args { get; internal set; }
    public string ProxiesFile { get; set; }
}

