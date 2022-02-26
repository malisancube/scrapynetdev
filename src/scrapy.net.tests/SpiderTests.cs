using System.Threading;
using Xunit;

namespace scrapy.net.tests;

public class SpiderTests
{
    [Fact]
    public void ScrapyApplication_MapsSpiders()
    {
        var builder = ScrapyApplication.CreateBuilder();
        var app = builder.Build();
        app.MapSpider("test");
        Assert.Contains(app.Spiders, s => s.Name == "test");
    }
}


public class TestSpider : Spider<IResponse>
{
    public override string Name => "test";

    public override string StartUrl => throw new NotImplementedException();

    public override Task<object?> ParseAsync(BaseRequest response, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<object?> StartRequestsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
