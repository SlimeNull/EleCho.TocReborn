using Newtonsoft.Json;

namespace NullLib.TocReborn
{
    public class TocBinaryMessage : TocMessage
    {
        public byte[]? Content { get; set; }

        public override TocMessageKind MessageKind => TocMessageKind.Binary;

        public TocBinaryMessage(byte[]? content) => Content = content;

        public TocBinaryMessage()
        {
        }
    }
}
