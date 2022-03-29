namespace NullLib.TocReborn.Pro
{
    public class TocProAccountVerifyInteractMessage : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.AccountVerifyInteract;

        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? VerificationCode { get; set; }

        public TocProAccountVerifyInteractMessage()
        {
        }

        public TocProAccountVerifyInteractMessage(string? account, string? password, string? verificationCode)
        {
            Account = account;
            Password = password;
            VerificationCode = verificationCode;
        }

        public const int ErrorNotRequestedOrExpired = -1;
        public const int ErrorInvalidCode = -2;
    }
}
