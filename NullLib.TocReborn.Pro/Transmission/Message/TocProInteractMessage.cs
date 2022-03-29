namespace NullLib.TocReborn.Pro
{
    public abstract class TocProInteractMessage : TocProMessage
    {
        public virtual TocProMessageResponse? Response { get; set; }
    }
    public class TocProEmptyInteractMessage : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.Interact;
    }
}
