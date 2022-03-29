namespace NullLib.TocReborn.Pro
{
    public delegate TocProMessage? TocProInteractMessageHandler(TocProConnection conn, TocProMessage request);

    public class TocProConnection : TocConnection
    {
        public TocProConnection(Stream stream) : base(stream) { }

        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(3);
        
        public void SendPro(TocProMessage msg)
        {
            SendProAsync(msg).Wait();
        }

        public Task SendProAsync(TocProMessage msg)
        {
            TocPackage? package = TocProMessage.BuildPackage(msg);
            if (package == null)
                throw new ArgumentException("Invalid message", nameof(msg));
            return SendPackageAsync(package);
        }

        public TocProMessage ReceivePro()
        {
            return ReceiveProAsync().Result;
        }

        public async Task<TocProMessage> ReceiveProAsync()
        {
            while (true)
            {
                if (TocProMessage.FromPackage(await ReceivePackageAsync()) is TocProMessage message)
                    return message;
            }
        }

        protected override void OnPackageReceived(TocPackage package)
        {
            base.OnPackageReceived(package);

            switch ((TocProPackageKind)package.Header.PackageKind)
            {
                case TocProPackageKind.Message:
                    if (TocProMessage.FromPackage(package) is TocProMessage message)
                        OnProMessageReceived(message);
                    break;
                case TocProPackageKind.InteractMessage:
                    if (TocProMessage.FromPackage(package) is TocProInteractMessage messageReq)
                    {
                        if (messageReq.Response == null)
                            OnProMessageRequestReceived(messageReq, package);
                        else
                            OnProMessageResponseReceived(messageReq, package);
                    }
                    break;
            }
        }

        protected virtual void OnProMessageReceived(TocProMessage message)
        {
            ProMessageReceived?.Invoke(this, new ProMessageEventArgs(message));
        }

        protected virtual void OnProMessageRequestReceived(TocProMessage message, TocPackage basePackage)
        {
            bool handled = false;
            foreach (var handler in requestHandlers)
            {
                if (handler.Invoke(this, message) is TocProMessage messageResp)
                {
                    _ = InternalSendPackageAsync(TocProMessage.BuildResponsePackage(basePackage, messageResp));
                    handled = true;
                }
            }

            if (!handled)
            {
                _ = InternalSendPackageAsync(TocProMessage.BuildResponsePackage(basePackage, TocProEmptyMessage.Empty));
            }

            ProMessageRequestReceived?.Invoke(this, new ProMessageEventArgs(message));
        }

        protected virtual void OnProMessageResponseReceived(TocProMessage message, TocPackage basePackage)
        {
            (AutoResetEvent waiter, TocProMessage? response) item = responses[basePackage.Header.MessageGuid];
            (AutoResetEvent waiter, TocProMessage? response) newVal = (item.waiter, message);
            responses[basePackage.Header.MessageGuid] = newVal;
            newVal.waiter.Set();

            ProMessageResponseReceived?.Invoke(this, new ProMessageEventArgs(message));
        }

        public event EventHandler<ProMessageEventArgs>? ProMessageReceived;

        public event EventHandler<ProMessageEventArgs>? ProMessageRequestReceived;

        public event EventHandler<ProMessageEventArgs>? ProMessageResponseReceived;

        private Dictionary<Guid, (AutoResetEvent waiter, TocProMessage? response)> responses = new Dictionary<Guid, (AutoResetEvent waiter, TocProMessage? response)>();
        private List<TocProInteractMessageHandler>requestHandlers = new List<TocProInteractMessageHandler>();

        public TocProMessage? InteractRequest(TocProMessage requestMessage)
        {
            return InteractRequestAsync(requestMessage).Result;
        }

        public async Task<TocProMessage?> InteractRequestAsync(TocProMessage requestMessage)
        {
            AutoResetEvent waiter = new AutoResetEvent(false);
            TocPackage packageReq = TocProMessage.BuildPackage(requestMessage);
            responses[packageReq.Header.MessageGuid] = (waiter, null);
            await InternalSendPackageAsync(packageReq);

            try
            {
                Guid reqGuid = packageReq.Header.MessageGuid;
                waiter.WaitOne(WaitTimeout);

                TocProMessage? response = responses[reqGuid].response;
                responses.Remove(reqGuid);

                return response;
            }
            catch
            {
                return null;
            }
        }

        public void SubcribeInteract(TocProInteractMessageHandler handler)
        {
            requestHandlers.Add(handler);
        }

        public void UnsubcribeInteract(TocProInteractMessageHandler handler)
        {
            requestHandlers.Remove(handler);
        }
    }
}
