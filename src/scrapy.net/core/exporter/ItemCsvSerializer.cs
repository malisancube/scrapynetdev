using CsvHelper;
using System.Globalization;

namespace scrapy.net;

/// <summary>
/// Csv serializer
/// </summary>
public class ItemCsvSerializer : IItemSerializer
{
	private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

	private string fileName = string.Empty;

	// Indicates if there is need to Flush to after each line to the <see cref="Outputfile"/>
	public bool ImmediateFlush { get; } = false;

	public ItemCsvSerializer(string fileName)
	{
		this.fileName = fileName;
	}

	/// <summary>
	/// Dispose item
	/// </summary>
	public void Dispose()
	{
		_readWriteLock.Dispose();
	}

	/// <summary>
	/// Serialize items as CSV 
	/// </summary>
	/// <param name="items"></param>
	public void Serialize(IEnumerable<object> items)
	{
		_readWriteLock.EnterWriteLock();
		try
		{
			using (var stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			using (var writer = new StreamWriter(stream))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(items);
			}
		}
		catch (FileNotFoundException ex)
		{
			System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
		}
		catch (IOException ex)
		{
			System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
		}
		catch (UnauthorizedAccessException ex)
		{
			System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
		}
		finally
		{
			// Release lock
			_readWriteLock.ExitWriteLock();
		}
	}

	/// <summary>
	/// Serialize item as Csv
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public string Serialize(object item)
	{
		Serialize((IEnumerable<object>)item);
		return string.Empty;
	}
}