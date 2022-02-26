namespace scrapy.net;

/// <summary>
/// Base Item serializer 
/// </summary>
public interface IItemSerializer
{
	bool ImmediateFlush { get; }
	void Dispose();
	string Serialize(object item);
}