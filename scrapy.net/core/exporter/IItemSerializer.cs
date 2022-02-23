namespace scrapy.net;

public interface IItemSerializer
{
	bool ImmediateFlush { get; }
	void Dispose();
	string Serialize(object item);
}