namespace scrapy.net;

public static class DictionaryUtils
{
    public static Dictionary<string, string> Merge(IEnumerable<Dictionary<string, string>> dictionaries)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = new Dictionary<string, string>(comparer);
        foreach (var dict in dictionaries)
            foreach (var x in dict)
                result[x.Key] = x.Value;
        return result;
    }
}