using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NullLib.TocReborn
{
    public class TocListener : TcpListener
    {
        public TocListener(IPEndPoint localEP) : base(localEP)
        {
        }

        public TocListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        public static new TocListener Create(int port) => new TocListener(IPAddress.Any, port);

        public TocConnection AcceptTocConnection() => new TocConnection(AcceptTcpClient().GetStream());

        public async Task<TocConnection> AcceptTocConnectionAsync() => new TocConnection((await AcceptTcpClientAsync()).GetStream());

        public IAsyncResult BeginAcceptTocConnection(AsyncCallback? callback, object? state) => BeginAcceptTcpClient(callback, state);

        public TocConnection EndAcceptTocConnection(IAsyncResult asyncResult) => new TocConnection(EndAcceptTcpClient(asyncResult).GetStream());
    }
}
