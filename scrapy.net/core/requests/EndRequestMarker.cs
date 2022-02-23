namespace scrapy.net;

public class EndRequestMarker
{
    public EndRequestMarker(Statistics statistics)
    {
        Statistics = statistics;
    }

    public Statistics Statistics { get; }
}

