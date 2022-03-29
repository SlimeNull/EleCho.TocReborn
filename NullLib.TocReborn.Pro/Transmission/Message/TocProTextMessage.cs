namespace NullLib.TocReborn.Pro
{
    public class TocProTextMessage : TocProMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.Text;

        public string? Text { get; set; }

        public TocProTextMessage(string text) => Text = text;

        public TocProTextMessage()
        {
        }
    }
}
