namespace scrapy.net;

public class EngineStartedEventArgs
{
    public EngineStartedEventArgs(ScrapyApplication scrapyApplication, IEnumerable<Spider<IResponse>> spiders)
    {
        ScrapyApplication = scrapyApplication;
        Spiders = spiders;
    }

    public ScrapyApplication ScrapyApplication { get; }
    public IEnumerable<Spider<IResponse>> Spiders { get; }
}