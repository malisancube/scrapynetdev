using System.Text.Json;

namespace scrapy.net;

public static class ObjectUtils
{
	public static R DeepClone<T, R>(T a)
	{
		var @object = JsonSerializer.Serialize(a);
		return JsonSerializer.Deserialize<R>(@object);
	}
}