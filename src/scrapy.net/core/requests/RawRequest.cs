using Microsoft.Extensions.Options;

namespace scrapy.net;

//public class RawRequest : BaseRequest
//{
//    public string Data { get; set; }

//    private readonly HttpClient _httpClient;

//    public RawRequest(IHttpClientFactory httpClientFactory, IOptions<DefaultSettings> settings) : base(httpClientFactory, settings)
//    {
//        _httpClient = httpClientFactory.CreateClient();

//        foreach (var header in settings.Value.DefaultRequestHeaders)
//        {
//            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
//        }
//    }

//    public override Task<IEnumerable<IResponse>> ExecuteAsync()
//    {
//        throw new NotImplementedException();
//    }
//}

