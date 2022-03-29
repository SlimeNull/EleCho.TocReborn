using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace NullLib.TocReborn
{
    public abstract class TocMessage
    {
        JsonSerializer serializer = JsonSerializer.Create(
            new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });


        public static Dictionary<TocMessageKind, Func<TocMessage>> MessageBuilders { get; } = new Dictionary<TocMessageKind, Func<TocMessage>>()
        {
            {TocMessageKind.Empty, () => new TocEmptyMessage()},
            {TocMessageKind.Text, () => new TocTextMessage()},
            {TocMessageKind.Binary, () => new TocBinaryMessage()},
        };

        [JsonIgnore]
        public abstract TocMessageKind MessageKind { get; }

        public static TocPackage BuildPackage(TocMessage msg)
        {
            MemoryStream ms = new MemoryStream();
            msg.SaveContent(ms);

            byte[] payload = ms.ToArray();
            return new TocPackage(
                new TocPackageHeader(TocPackageKind.Message, msg.MessageKind, (uint)payload.LongLength),
                payload);
        }

        public static TocMessage? FromPackage(TocPackage? package)
        {
            if (package == null)
                return null;
            if (package.Content == null)
                return null;
            if (!MessageBuilders.TryGetValue(package.Header.MessageKind, out Func<TocMessage>? builder))
                return null;

            TocMessage msg = builder.Invoke();
            msg.LoadContent(new MemoryStream(package.Content));
            return msg;
        }

        public virtual void LoadContent(Stream stream)
        {
            using var reader = new BsonReader(stream);
            serializer.Populate(reader, this);
        }
        public virtual void SaveContent(Stream stream)
        {
            using var writer = new BsonWriter(stream);
            serializer.Serialize(writer, this);
        }
    }
}