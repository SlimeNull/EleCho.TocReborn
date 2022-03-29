using NullLib.ConsoleEx;
using NullLib.TocReborn;
using NullLib.TocReborn.Pro;
using System;
using System.Net;
using System.Net.Sockets;

namespace TestClient
{
    public class Program
    {
        static Program()
        {
            ConsoleSc.Prompt = ">>> ";
            TocProMessage.DefaultSender = "114514";
        }

        static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();

            Console.Write("Account: ");
            TocProMessage.DefaultSender = Console.ReadLine();
            Console.Write("Address: ");
            string? input = Console.ReadLine();

            if (input == null)
                return;
            tcpClient.Connect(IPEndPoint.Parse(input));
            var connection = new TocProConnection(tcpClient.GetStream());
            connection.StartReceive();
            connection.ProMessageReceived += Connection_MessageReceived;

            while (true)
            {
                input = ConsoleSc.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    TocProMessage? tocProMessage = connection.InteractRequest(new TocProEmptyInteractMessage());
                    if (tocProMessage is TocProInteractMessage resp)
                    {
                        Console.WriteLine(resp.Response?.Message);
                    }
                }
                else
                {
                    connection.Send(new TocProTextMessage(input));

                }
            }
        }

        private static void Connection_MessageReceived(object? sender, ProMessageEventArgs e)
        {
            if (e.Message == null)
                return;

            if (e.Message is TocProTextMessage textMessage)
            {
                if (textMessage.Sender == TocProMessage.DefaultSender)
                    return;
                ConsoleSc.WriteLine($"$>> {textMessage.Text}");
            }
        }
    }
}