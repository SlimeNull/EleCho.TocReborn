using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NullLib.TocReborn.Pro
{
    public static class WebSocketEx
    {
        public class WebSocketNetworkStream : Stream
        {
            public WebSocket BaseWebSocket { get; }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public WebSocketNetworkStream(WebSocket webSocket)
            {
                BaseWebSocket = webSocket;
            }

            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count)
            {
                return BaseWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, count), new CancellationToken()).Result.Count;
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                BaseWebSocket.SendAsync(new ArraySegment<byte>(buffer, offset, count), WebSocketMessageType.Binary, true, new CancellationToken()).Wait();
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
        }
        public static TocProConnection CreateTocProConnection(this WebSocket webSocket)
        {
            return new TocProConnection(new WebSocketNetworkStream(webSocket));
        }
    }
}
