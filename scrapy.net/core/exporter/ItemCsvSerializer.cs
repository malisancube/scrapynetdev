using CsvHelper;
using System.Globalization;

namespace scrapy.net;

public class ItemCsvSerializer : IItemSerializer
{
	private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

	private string fileName = string.Empty;

	public bool ImmediateFlush { get; } = false;

	public ItemCsvSerializer(string fileName)
	{
		this.fileName = fileName;
	}

	public void Dispose()
	{
		_readWriteLock.Dispose();
	}

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

	public string Serialize(object item)
	{
		Serialize((IEnumerable<object>)item);
		return string.Empty;
	}
}
