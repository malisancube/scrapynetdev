using RabbitMQ.Client;
using System.Text;

async IAsyncEnumerable<string> ReadInputAsync(string fileName)
{
    using (var reader = new StreamReader(fileName))
    {
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                break;
            }
            else
            {
                yield return line;
            }
        }
    }
}

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "scrape_urls", type: ExchangeType.Fanout);

    await foreach (var line in ReadInputAsync(""))
    {
        var messagebuffer = Encoding.Default.GetBytes(line);
        channel.BasicPublish(exchange: "scrape_urls",
                    routingKey: "",
                    basicProperties: null,
                    body: messagebuffer);
        Console.WriteLine($"{line}...");
    }

}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
