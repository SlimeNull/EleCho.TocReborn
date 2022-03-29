namespace NullLib.TocReborn.Pro
{
    public class TocProImageMessage : TocProMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.Image;

        public string? ImageSource { get; set; }

        public TocProImageMessage(string imageSource) : base() => ImageSource = imageSource;

        public TocProImageMessage() : base()
        {
        }
    }
}
