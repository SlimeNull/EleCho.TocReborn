using System.Net;

namespace NullLib.TocReborn.Pro
{
    public class TocProListener : TocListener
    {
        public TocProListener(IPEndPoint localEP) : base(localEP)
        {
        }

        public TocProListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        public static new TocProListener Create(int port) => new TocProListener(IPAddress.Any, port);

        public TocProConnection AcceptTocProConnection() => new TocProConnection(AcceptTcpClient().GetStream());

        public async Task<TocProConnection> AcceptTocProConnectionAsync() => new TocProConnection((await AcceptTcpClientAsync()).GetStream());

        public IAsyncResult BeginAceeptTocProConnection(AsyncCallback? callback, object? state) => BeginAcceptTcpClient(callback, state);

        public TocProConnection EndAcceptTocProConnection(IAsyncResult asyncResult) => new TocProConnection(EndAcceptTcpClient(asyncResult).GetStream());
    }
}
