using scrapy.net;
using Serilog;

try
{
    var builder = ScrapyApplication.CreateBuilder(args);
    var app = builder.Build();
    app.MapSpiders();
    await app.RunAsync();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}