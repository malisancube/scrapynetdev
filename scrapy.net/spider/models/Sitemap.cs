using System.Collections;

namespace scrapy.net;

public class Sitemap : IEnumerable<KeyValuePair<string, object>>
{
    private readonly string text;

    public Sitemap(string text)
    {
        this.text = text;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}