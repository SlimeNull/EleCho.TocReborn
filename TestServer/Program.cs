using NullLib.TocReborn;
using NullLib.TocReborn.Pro;
using System;
using System.Net.Sockets;

namespace TestServer
{
    public class Program
    {
        static List<TocConnection> connections = new List<TocConnection>();
        static void Main(string[] args)
        {
            var listener = new TocProListener(System.Net.IPAddress.Any, 5555);
            listener.Start();

            Console.WriteLine("Listening port: 5555");

            while(true)
            {
                var connection = listener.AcceptTocProConnection();
                connections.Add(connection);
                connection.StartReceive();
                connection.SubcribeInteract(RequestHandler);
                connection.ProMessageReceived += Connection_MessageReceived;
            }
          }

        private static TocProMessage? RequestHandler(TocProConnection conn, TocProMessage request)
        {
            return new TocProEmptyInteractMessage()
            {
                Response = new TocProMessageResponse()
                {
                    Message = "Fuck you world"
                }
            };
        }

        private static void Connection_MessageReceived(object? sender, ProMessageEventArgs e)
        {
            if (e.Message == null)
                return;

            foreach (TocConnection connection in connections)
            {
                connection.Send(e.Message);
            }
        }
    }
}