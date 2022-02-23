namespace scrapy.net;

public class Statistics
{
    private static object _lock = new object();
    private static Statistics _instance = new Statistics();

    public static Statistics Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = new Statistics();
                return _instance;
            }
        }
    }

    public int Pages { get; set; }
    public int Items { get; internal set; }
}