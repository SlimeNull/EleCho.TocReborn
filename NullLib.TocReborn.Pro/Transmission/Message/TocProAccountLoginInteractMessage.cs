namespace NullLib.TocReborn.Pro
{

    public class TocProAccountLoginInteractMessage : TocProInteractMessage
    {
        public const int ErrorInvalidAccount = -1;
        public const int ErrorInvalidPassword = -2;

        public override TocProMessageKind ProMessageKind => TocProMessageKind.AccountLoginInteract;

        public TocProAccountLoginInteractMessage() : base()
        {
        }

        public TocProAccountLoginInteractMessage(string account, string password) : base()
        {
            Account = account;
            Password = password;
        }

        public string? Account { get; set; }
        public string? Password { get; set; }
    }
}
