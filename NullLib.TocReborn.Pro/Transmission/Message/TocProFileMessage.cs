namespace NullLib.TocReborn.Pro
{
    public class TocProFileMessage : TocProMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.File;

        public TocProFileMessage() : base()
        {
        }

        public TocProFileMessage(string fileName, string fileSource) : base()
        {
            FileName = fileName;
            FileSource = fileSource;
        }

        public string? FileName { get; set; }
        public string? FileSource { get; set; }
    }
}
