namespace scrapy.net;

//public class GraphQLRequest : BaseRequest
//{
//    public string Query { get; set; }
//    public string Variables { get; set; }

//    private readonly HttpClient _httpClient;

//    public GraphQLRequest(IHttpClientFactory httpClientFactory, IOptions<DefaultSettings> settings) : base(httpClientFactory, settings)
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

