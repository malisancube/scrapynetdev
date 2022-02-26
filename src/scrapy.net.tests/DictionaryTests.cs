using Xunit;

namespace scrapy.net.tests;

public class DictionaryTests
{
    [Fact]
    public void Dictionary_Merge()
    {
        Dictionary<string, string> map1 = new()
        {
            ["key1"] = "value1"
        };
        Dictionary<string, string> map2 = new()
        {
            ["key2"] = "value2"
        };
        var all = DictionaryUtils.Merge(new[] { map1, map2 });

        Assert.Equal(2, all.Count());
    }
}
