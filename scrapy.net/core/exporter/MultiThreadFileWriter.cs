using System.Collections.Concurrent;

namespace scrapy.net;

public class MultiThreadFileWriter
{
	private static ConcurrentQueue<string> _textToWrite = new ConcurrentQueue<string>();
	private readonly string fileName;
	private CancellationTokenSource _source = new CancellationTokenSource();
	private CancellationToken _token;
	private bool _lineBreak;

	public MultiThreadFileWriter(string fileName, bool lineBreak = true)
	{
		_token = _source.Token;
		_lineBreak = lineBreak;
		// This is the task that will run
		// in the background and do the actual file writing
		Task.Run(WriteToFile, _token);
		this.fileName = fileName;
	}

	/// The public method where a thread can ask for a line
	/// to be written.
	public void WriteLine(string line)
	{
		_textToWrite.Enqueue(line);
	}

	/// The actual file writer, running
	/// in the background.
	private async void WriteToFile()
	{
		while (true)
		{
			if (_token.IsCancellationRequested)
			{
				return;
			}
			using (var w = File.AppendText(fileName))
			{
				while (_textToWrite.TryDequeue(out string textLine))
				{
					if (_lineBreak)
						await w.WriteLineAsync(textLine);
					else
						await w.WriteAsync(textLine);
				}
				w.Flush();
			}
		}
	}
}