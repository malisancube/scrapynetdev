namespace scrapy.net;

public class ItemXmlSerializer : IItemSerializer
{
	private string Header = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<items>\n";
	private string Footer = "</items>";
	private bool Initialized = false;
	private MultiThreadFileWriter writer;

	public bool ImmediateFlush { get; }

	public ItemXmlSerializer(bool immediateFlush = true, MultiThreadFileWriter writer = null)
	{
		this.writer = writer;
		ImmediateFlush = immediateFlush;
	}

	public void Dispose()
	{
		writer.WriteLine(Footer);
	}

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
