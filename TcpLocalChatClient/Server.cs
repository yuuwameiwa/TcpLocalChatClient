using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpLocalChatClient
{
    internal class Server
    {
        private string Host = "127.0.0.1";
        private int port = 8888;

        private Client _client = new Client();

        private TcpClient _tcpClient = new TcpClient();
        private StreamReader? _reader = null;
        private StreamWriter? _writer = null;

        public async void Start()
        {
            Console.WriteLine("Введите имя:");
            _client.UserName = Console.ReadLine();

            _tcpClient.Connect(Host, port);
            _reader = new StreamReader(_tcpClient.GetStream());
            _writer = new StreamWriter(_tcpClient.GetStream());

            if (_reader is null || _writer is null)
                return;

            Task.Run(() => ReceiveMessageAsync(_reader));

            await SendMessageAsync(_writer);
        }

        private async Task ReceiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    string? message = await reader!.ReadLineAsync();

                    if (string.IsNullOrEmpty(message))
                        continue;

                    Print(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task SendMessageAsync(StreamWriter writer)
        {
            while (true)
            {
                try
                {
                    await writer.WriteLineAsync(_client.UserName);
                    await writer.FlushAsync();

                    while (true)
                    {
                        string? message = Console.ReadLine();
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Print(string message)
        {
            if (OperatingSystem.IsWindows())
            {
                var position = Console.GetCursorPosition();

                int left = position.Left;
                int top = position.Top;

                Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
                Console.SetCursorPosition(0, top);
                Console.WriteLine(message);
                Console.SetCursorPosition(left, top + 1);
            }
            else
                Console.WriteLine(message);
        }
    }
}
