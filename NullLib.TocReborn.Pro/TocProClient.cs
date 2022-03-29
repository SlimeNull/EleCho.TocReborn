using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullLib.TocReborn.Pro
{
    public class TocProClient
    {
        private readonly TocProConnection baseConnection;
        public TocProConnection BaseConnection => baseConnection;

        public TocProClient(TocProConnection connection)
        {
            baseConnection = connection;
            baseConnection.ProMessageReceived += ProcConnMessage;
        }

        

        private void ProcConnMessage(object? sender, ProMessageEventArgs e)
        {
            if (e.Message == null)
                return;

            switch (e.Message.ProMessageKind)
            {
                case TocProMessageKind.Text:
                case TocProMessageKind.Image:
                case TocProMessageKind.File:
                    OnMessageReceived(e.Message);
                    break;
            }
        }

        /// <summary>
        /// 处理服务端向客户端发起的交互消息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private TocProMessage? ProcConnRequest(TocProConnection connection, TocProMessage request)
        {
            return null;
        }

        void SubcribeConnEvents()
        {
            baseConnection.SubcribeInteract(ProcConnRequest);
            baseConnection.ProMessageReceived += ProcConnMessage;
        }

        void UnsubcribeConnEvents()
        {
            baseConnection.UnsubcribeInteract(ProcConnRequest);
            baseConnection.ProMessageReceived -= ProcConnMessage;
        }

        public void Start()
        {
            SubcribeConnEvents();
            baseConnection.StartReceive();
        }

        public void Stop()
        {
            UnsubcribeConnEvents();
            baseConnection.StopReceive();
        }

        public void Login(string account, string password)
        {

        }

        public void LoginAsync(string account, string password)
        {

        }

        public void CreateAccount(string account, string password)
        {

        }

        public void VerifyAccount(string verificationCode)
        {

        }

        protected virtual void OnMessageReceived(TocProMessage msg)
        {
            MessageReceived?.Invoke(this, new ProMessageEventArgs(msg));
        }

        ~TocProClient()
        {
            Stop();
        }

        public event EventHandler<ProMessageEventArgs>? MessageReceived;
    }
}
