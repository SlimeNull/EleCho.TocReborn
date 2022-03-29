namespace NullLib.TocReborn.Pro
{
    public class TocProChannelCreateInteractMessage : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.ChannelCreateInteract;

        public const int ErrorCannotCreate = -1;
    }
}
