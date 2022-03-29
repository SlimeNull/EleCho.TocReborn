using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullLib.TocReborn.Pro
{
    public abstract class TocProMessage : TocMessage
    {
        private string? sender;
        private string? sendTo;
        private static string? defaultSender;
        private static string? defaultSendTo;

        public static string? DefaultSender { get => defaultSender; set => defaultSender = value; }
        public static string? DefaultSendTo { get => defaultSendTo; set => defaultSendTo = value; }

        public static Dictionary<TocProMessageKind, Func<TocProMessage>> ProMessageBuilders { get; } = new Dictionary<TocProMessageKind, Func<TocProMessage>>()
        {
            { TocProMessageKind.Empty, () => new TocProEmptyMessage() },
            { TocProMessageKind.Text, () => new TocProTextMessage() },
            { TocProMessageKind.Image, () => new TocProImageMessage() },
            { TocProMessageKind.File, () => new TocProFileMessage() },

            { TocProMessageKind.Interact, () => new TocProEmptyInteractMessage() },
            { TocProMessageKind.AccountCreateInteract, () => new TocProAccountCreateInteractMessage() },
            { TocProMessageKind.AccountLoginInteract, () => new TocProAccountLoginInteractMessage() },
        };
        public abstract TocProMessageKind ProMessageKind { get; }
        public override TocMessageKind MessageKind => (TocMessageKind)ProMessageKind;

        public string? Sender
        {
            get => sender ?? defaultSender; set
            {
                if (defaultSender == null)
                    defaultSender = value;
                sender = value;
            }
        }

        public string? SendTo
        {
            get => sendTo ?? defaultSendTo; set
            {
                if (defaultSendTo == null)
                    defaultSendTo = value;
                sendTo = value;
            }
        }

        public static TocPackage BuildPackage(TocProMessage msg)
        {
            MemoryStream ms = new MemoryStream();
            msg.SaveContent(ms);

            TocPackageKind packageKind = msg switch
            {
                TocProInteractMessage imsg when imsg.Response == null => (TocPackageKind)TocProPackageKind.InteractMessage,
                TocProInteractMessage imsg when imsg.Response != null => throw new ArgumentException(
                    $"Please use the method '{nameof(BuildResponsePackage)}' to build.", nameof(imsg)),

                _ => (TocPackageKind)TocProPackageKind.Message
            };

            byte[] payload = ms.ToArray();
            return new TocPackage(new TocPackageHeader(packageKind, msg.MessageKind, (uint)payload.LongLength), payload);
        }

        public static TocPackage BuildResponsePackage(TocPackage request, TocProMessage msg)
        {
            MemoryStream ms = new MemoryStream();
            msg.SaveContent(ms);

            byte[] payload = ms.ToArray();
            return new TocPackage(
                new TocPackageHeader(
                    (TocPackageKind)TocProPackageKind.InteractMessage,
                    msg.MessageKind,
                    (uint)payload.LongLength,
                    request.Header.MessageGuid),
                payload);
        }

        public static new TocProMessage? FromPackage(TocPackage package)
        {
            if (package == null)
                return null;
            if (package.Content == null)
                return null;
            if (!ProMessageBuilders.TryGetValue((TocProMessageKind)package.Header.MessageKind, out Func<TocProMessage>? builder))
                return null;

            TocProMessage msg = builder.Invoke();
            msg.LoadContent(new MemoryStream(package.Content));
            return msg;
        }
    }
    public class TocProEmptyMessage : TocProMessage
    {

        public TocProEmptyMessage(string? sender, string? sendTo)
        {
            Sender = sender;
            SendTo = sendTo;
        }

        public TocProEmptyMessage()
        {
        }

        [JsonIgnore]
        public override TocProMessageKind ProMessageKind => TocProMessageKind.Empty;

        public static TocProMessage Empty => new TocProEmptyMessage();
    }
}
