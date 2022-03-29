namespace NullLib.TocReborn.Pro
{
    public class TocProAccountModifyInteractMessage : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.AccountModifyInteract;

        public TocProUser NewInfo { get; set; }

        public TocProAccountModifyInteractMessage(TocProUser newInfo) => NewInfo = newInfo;

        public TocProAccountModifyInteractMessage()
        {
        }
    }

    public class TocProChannelModifyInteractMessage : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.ChannelModifyInteract;
    }
}
