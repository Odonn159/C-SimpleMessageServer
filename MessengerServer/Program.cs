using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static TcpListener _listener;
    private static List<TcpClient> _clients = new List<TcpClient>();

    static async Task Main(string[] args)
    {
        int port = 12345; // Port for the server
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        Console.WriteLine($"Server started on port {port}.");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _clients.Add(client);
            Console.WriteLine("Client connected.");
            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        using (var stream = client.GetStream())
        {
            var buffer = new byte[1024];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                // Broadcast message to all connected clients
                foreach (var c in _clients)
                {
                    if (c != client)
                    {
                        var data = Encoding.UTF8.GetBytes(message);
                        await c.GetStream().WriteAsync(data, 0, data.Length);
                    }
                }
            }
        }

        _clients.Remove(client);
        client.Close();
        Console.WriteLine("Client disconnected.");
    }
}