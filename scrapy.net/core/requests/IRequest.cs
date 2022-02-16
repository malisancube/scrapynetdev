using System.Runtime.CompilerServices;

namespace scrapy.net;

public interface IRequest
{
    Task<object?> ExecuteAsync([CallerMemberName] string callerName = "");
}