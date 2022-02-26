namespace scrapy.net;

/// <summary>
/// Xml serializer
/// </summary>
public class ItemXmlSerializer : IItemSerializer
{
	private string Header = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<items>\n";
	private string Footer = "</items>";
	private bool Initialized = false;
	private MultiThreadFileWriter writer;

	// Indicates if there is need to Flush to after each line to the <see cref="Outputfile"/>
	public bool ImmediateFlush { get; }

	public ItemXmlSerializer(bool immediateFlush = true, MultiThreadFileWriter writer = null)
	{
		this.writer = writer;
		ImmediateFlush = immediateFlush;
	}

	/// <summary>
	/// Dispose item
	/// </summary>
	public void Dispose()
	{
		writer.WriteLine(Footer);
	}

	/// <summary>
	/// Serialize item as XML
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public string Serialize(object item)
	{
		string itemString = item.ToXml().ToString();
		if (!Initialized)
		{
			itemString = Header + itemString;
			Initialized = true;
		}
		writer.WriteLine(itemString);
		return null;
	}
}