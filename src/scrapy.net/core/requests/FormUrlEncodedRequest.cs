namespace scrapy.net;

//public class FormUrlEncodedRequest : BaseRequest
//{
//    public PropertyBag FormData { get; set; }

//    private readonly HttpClient _httpClient;

//    public FormUrlEncodedRequest(IHttpClientFactory httpClientFactory, IOptions<DefaultSettings> settings) : base(httpClientFactory, settings)
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

