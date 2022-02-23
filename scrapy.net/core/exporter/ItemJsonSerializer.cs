using System.Text.Json;

namespace scrapy.net;

public class ItemJsonSerializer : IItemSerializer
{
	private MultiThreadFileWriter writer;

	public bool ImmediateFlush { get; }

	public ItemJsonSerializer(bool immediateFlush, MultiThreadFileWriter writer)
	{
		this.writer = writer;
		this.ImmediateFlush = immediateFlush;
	}

	public void Dispose()
	{
	}

	public string Serialize(object item)
	{
		var json = JsonSerializer.Serialize(item);
		writer.WriteLine(json);
		return String.Empty;
	}

	public string Serialize(StreamWriter item)
	{
		return JsonSerializer.Serialize(item);
	}
}