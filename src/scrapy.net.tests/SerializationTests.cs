using Xunit;

namespace scrapy.net.tests;

public class SerializationTests
{
    [Fact]
    public void Serialization_Clone()
    {
        var applicationSettings = new ApplicationSettings();
        var spiderSettings = ObjectUtils.DeepClone<ApplicationSettings, SpiderSettings>(applicationSettings);

        Assert.Equal(applicationSettings.BotName, spiderSettings.BotName);
        Assert.Equal(applicationSettings.EnvironmentName, spiderSettings.EnvironmentName);
        Assert.Equal(applicationSettings.DefaultOutputFileExtention, spiderSettings.DefaultOutputFileExtention);
    }
}
