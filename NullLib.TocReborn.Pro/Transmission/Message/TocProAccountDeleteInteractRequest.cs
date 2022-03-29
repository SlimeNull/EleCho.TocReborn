namespace NullLib.TocReborn.Pro
{
    public class TocProAccountDeleteInteractRequest : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.AccountDeleteInteract;
        public string? Account { get; set; }

        public TocProAccountDeleteInteractRequest(string? account) => Account = account;

        public TocProAccountDeleteInteractRequest()
        {
        }

        public const int ErrorAccountNotExist = -1;
    }
}
