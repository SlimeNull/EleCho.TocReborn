using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NullLib.TocReborn
{
    public class TocConnection : IDisposable
    {
        readonly Stream baseStream;
        bool isConnected = true;
        bool loopRecv = false;
        Task? recvLoopTask = null;
        public Stream BaseStream => baseStream;
        public bool IsConnected => isConnected;
        public bool LoopReceive => loopRecv;

        public TocConnection(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("Stream cannot write", nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Stream cannot read", nameof(stream));

            baseStream = stream;
        }

        async Task StartupCheck()
        {
            bool ok = await SendPing();
            if (ok)
                OnConnectionOpened();
            isConnected = ok;
        }

        internal async Task<bool> SendPing()
        {
            try
            {
                await InternalSendPackageAsync(TocPackage.CreatePing(false));
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        protected internal async Task<TocPackage?> InternalReceivePackageAsync()
        {
            try
            {
                byte[] headerBuffer = new byte[TocPackageHeader.HeaderByteLength];
                await baseStream.ReadAsync(headerBuffer, 0, TocPackageHeader.HeaderByteLength);
                TocPackageHeader header = TocPackageHeader.FromBytes(headerBuffer, 0);
                byte[] messageBuffer = new byte[header.MessageLength];
                await baseStream.ReadAsync(messageBuffer, 0, messageBuffer.Length);

                return new TocPackage(header, messageBuffer);
            }
            catch (ObjectDisposedException)
            {
                OnConnectionClosed();
                return null;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected internal async Task InternalSendPackageAsync(TocPackage package)
        {
            try
            {
                byte[] headerbs = TocPackageHeader.GetBytes(package.Header);
                await baseStream.WriteAsync(headerbs, 0, TocPackageHeader.HeaderByteLength);
                await baseStream.WriteAsync(package.Content, 0, package.Content.Length);
            }
            catch (ObjectDisposedException)
            {
                OnConnectionClosed();
                throw new InvalidOperationException("Connection is closed");
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task ReceivePackageLoop()
        {
            while(isConnected && loopRecv)
            {
                if (await InternalReceivePackageAsync() is TocPackage tocPackage)
                    OnPackageReceived(tocPackage);
            }
        }

        public void StartReceive()
        {
            loopRecv = true;
            recvLoopTask = ReceivePackageLoop();
        }

        public void StopReceive()
        {
            loopRecv = false;
            if (recvLoopTask != null)
                recvLoopTask.Wait();
            recvLoopTask = null;
        }

        public TocPackage ReceivePackage()
        {
            if (loopRecv)
                throw new InvalidOperationException("Loop receiving now.");

            return InternalReceivePackageAsync().Result;
        }

        public async Task<TocPackage> ReceivePackageAsync()
        {
            if (loopRecv)
                throw new InvalidOperationException("Loop receiving now.");

            return await InternalReceivePackageAsync();
        }

        public void SendPackage(TocPackage package)
        {
            InternalSendPackageAsync(package).Wait();
        }

        public Task SendPackageAsync(TocPackage package)
        {
            return InternalSendPackageAsync(package);
        }

        public TocMessage Receive()
        {
            return ReceiveAsync().Result;
        }

        public async Task<TocMessage> ReceiveAsync()
        {
            while (true)
            {
                if (TocMessage.FromPackage(await ReceivePackageAsync()) is TocMessage message)
                    return message;
            }
        }

        public void Send(TocMessage msg)
        {
            SendAsync(msg).Wait();
        }

        public Task SendAsync(TocMessage msg)
        {
            TocPackage? package = TocMessage.BuildPackage(msg);
            if (package == null)
                throw new ArgumentException("Invalid message", nameof(msg));
            return SendPackageAsync(package);
        }

        protected virtual void OnPackageReceived(TocPackage package)
        {
            PackageReceived?.Invoke(this, new PackageEventArgs(package));

            switch (package.Header.PackageKind)
            {
                case TocPackageKind.Ping:
                    if (package.Content.Length > 0)
                        _ = InternalSendPackageAsync(TocPackage.CreatePing(true));
                    break;
                case TocPackageKind.Message:
                    if (TocMessage.FromPackage(package) is TocMessage message)
                        OnMessageReceived(message);
                    break;
            }
        }
        protected virtual void OnMessageReceived(TocMessage message)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs(message));
        }

        protected virtual void OnConnectionOpened()
        {
            ConnectionOpened?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnConnectionClosed()
        {
            if (isConnected)
                ConnectionClosed?.Invoke(this, EventArgs.Empty);
            isConnected = false;
        }

        public event EventHandler<PackageEventArgs>? PackageReceived;
        public event EventHandler<MessageEventArgs>? MessageReceived;

        public event EventHandler? ConnectionOpened;
        public event EventHandler? ConnectionClosed;

        public void Dispose()
        {
            baseStream.Dispose();
        }
    }
}
