using System.Text.Json;

namespace scrapy.net;

/// <summary>
/// 
/// </summary>
public class ItemJsonSerializer : IItemSerializer
{
	private MultiThreadFileWriter writer;

	// Indicates if there is need to Flush to after each line to the <see cref="Outputfile"/>
	public bool ImmediateFlush { get; }

	public ItemJsonSerializer(bool immediateFlush, MultiThreadFileWriter writer)
	{
		this.writer = writer;
		this.ImmediateFlush = immediateFlush;
	}

	/// <summary>
	/// Dispose item
	/// </summary>
	public void Dispose()
	{
	}

	/// <summary>
	/// Serialize item as Json
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public string Serialize(object item)
	{
		var json = JsonSerializer.Serialize(item);
		writer.WriteLine(json);
		return String.Empty;
	}

	/// <summary>
	/// Serialize item as Json
	/// </summary>
	/// <param name="item"></param>
	/// <returns>string</returns>
	public string Serialize(StreamWriter item)
	{
		return JsonSerializer.Serialize(item);
	}
}