using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace scrapy.net.tests;

public class ScrapyApplicationTests
{
    [Fact]
    public void ScrapyApplication_IncludesCommandLineArguments()
    {
        var builder = ScrapyApplication.CreateBuilder(new string[] { "--proxiesfile", "proxy.txt" });
        Assert.Equal("proxy.txt", builder.Configuration["proxiesfile"]);
    }

    [Fact]
    public void ScrapyApplication_DefaultConfiguration()
    {
        var builder = ScrapyApplication.CreateBuilder();
        Assert.Equal("scrapybot_test", builder.Configuration["BotName"]);
    }

    [Fact]
    public void ScrapyApplication_EnvironmentVariable()
    {
        var random = new Random();
        var number = random.Next(0,1000);
        Environment.SetEnvironmentVariable("BOTTEST", $"BOTTEST_{number}");
        var builder = ScrapyApplication.CreateBuilder();
        Assert.Equal("BOTTEST", builder.Configuration[$"BOTTEST_{number}"]);
        Environment.SetEnvironmentVariable("BOTTEST", "");
    }

    [Fact]
    public void ScrapyApplication_HasInitialServices()
    {
        var builder = ScrapyApplication.CreateBuilder();
        Assert.True(builder.Services.Any(), "Initial services registered");
    }

    [Fact]
    public void ScrapyApplication_HasDefaultProviders()
    {
        var builder = ScrapyApplication.CreateBuilder();
        Assert.True(builder.Configuration?.Providers.Count() > 1);
    }

    [Fact]
    public void ScrapyApplication_UpdatesSourceConfiguration()
    {
        var jsonFileSource = new JsonConfigurationSource
        {
            FileProvider = new NullFileProvider(),
            Path = ".",
            Optional = true,
            ReloadOnChange = false
        };

        var builder = ScrapyApplication.CreateBuilder();
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.Add(jsonFileSource);
        });
        Assert.Equal(1, builder.Configuration?.Providers.Count());
    }

    [Fact]
    public void ScrapyApplication_UsingLogging()
    {
        var builder = ScrapyApplication.CreateBuilder();
        builder.ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        var app = builder.Build();
        // TODO: Fix verification
        Assert.True(app.Logger != null);
    }

    [Fact]
    public void ScrapyApplication_HasNospiders()
    {
        var builder = ScrapyApplication.CreateBuilder();
        var app = builder.Build();
        Assert.True(app.Spiders.Any());
    }
}
