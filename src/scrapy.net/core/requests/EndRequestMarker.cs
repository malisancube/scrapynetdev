namespace scrapy.net;

/// <summary>
/// End request marker to enable the scraper to know when the end of the scraping session
/// </summary>
public class EndRequestMarker
{
    public EndRequestMarker(Statistics statistics)
    {
        Statistics = statistics;
    }

    public Statistics Statistics { get; }
}