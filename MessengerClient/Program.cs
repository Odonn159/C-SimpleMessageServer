// See https://aka.ms/new-console-template for more information
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static TcpClient _client;
    private static NetworkStream _stream;

    static async Task Main(string[] args)
    {
        string serverIp = "127.0.0.1"; // Server IP address
        int serverPort = 12345;        // Server port

        _client = new TcpClient();
        await _client.ConnectAsync(serverIp, serverPort);
        _stream = _client.GetStream();
        Console.WriteLine("Connected to server.");
        Console.WriteLine("Type 'quit' or 'q' to exit");
        _ = Task.Run(() => ReceiveMessagesAsync());

        while (true)
        {
            var message = Console.ReadLine();
            if (message == null) continue;
            if (message == "quit"){
                break;
            }
            if (message == "q"){
                break;
            }
            var data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }
    }

    private static async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024];
        while (true)
        {
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received: {message}");
        }
    }
}
