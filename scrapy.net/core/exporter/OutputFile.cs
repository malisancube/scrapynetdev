﻿using System.Collections.Concurrent;

namespace scrapy.net;

public class OutputFile : IDisposable
{
	public string FileName { get; }
	private IItemSerializer serializer;
	private ConcurrentBag<object> items = new();

	public OutputFile(string fileName)
	{
		FileName = CreateFileName(fileName);
		serializer = GetSerializer();
	}

	public string FullPathName => Path.GetFullPath(FileName);

	private IItemSerializer GetSerializer()
	{
		var type = Path.GetExtension(FileName);
		switch (type)
		{
			case ".jl": return new ItemJsonSerializer(immediateFlush: true, writer: new MultiThreadFileWriter(FileName));
			case ".json": return new ItemJsonSerializer(immediateFlush: false, writer: new MultiThreadFileWriter(FileName));
			case ".csv": return new ItemCsvSerializer(fileName: FileName);
			case ".xml": return new ItemXmlSerializer(immediateFlush: true, writer: new MultiThreadFileWriter(FileName));
			default:
				throw new Exception("Could not determine output file type.");
		}
	}

	private string CreateFileName(string fileName)
	{
		int count = 1;

		var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
		var extension = Path.GetExtension(fileName);
		var path = Path.GetDirectoryName(fileName);
		var newFullPath = fileName;

		while (File.Exists(newFullPath))
		{
			var tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
			newFullPath = Path.Combine(path, tempFileName + extension);
		}
		return newFullPath;
	}

	internal void Write(object item)
	{
		if (serializer.ImmediateFlush)
		{
			serializer.Serialize(item);
			return;
		}
		items.Add(item);
	}

	internal void Flush()
	{
		serializer.Serialize(items);
	}

	public void Dispose()
	{
		if (!serializer.ImmediateFlush)
		{
			Flush();
		}
		serializer.Dispose();
		items.Clear();
		GC.Collect();
	}
}
