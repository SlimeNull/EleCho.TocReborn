#if NET47
#endif

namespace NullLib.TocReborn.Pro
{
    /// <summary>
    /// TOC 账户
    /// </summary>
    public class TocProUser : TocProUserBase
    {
        public string? Password { get; set; }

        public TocProUser(string account, string password)
        {
            Account = account;
            Password = password;
        }
    }
}
