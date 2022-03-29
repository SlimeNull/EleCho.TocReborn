namespace NullLib.TocReborn.Pro
{
    public interface ITocProServerDataManager
    {
        IEnumerable<TocProUser> GetUsers();
        IEnumerable<TocProChannel> GetChannels();
        
        bool ContainsAccount(string account) => ContainsUser(account) || ContainsChannel(account);
        bool ContainsUser(string userAccount);
        bool ContainsChannel(string channelAccount);

        bool TryGetUser(string userAccount, out TocProUser user);
        bool TryGetChannel(string channelAccount, out TocProChannel channel);

        TocProUser GetUser(string userAccount);
        TocProChannel GetChannel(string channelAccount);

        void AddUser(TocProUser user);
        void AddChannel(TocProChannel channel);

        void RemoveUser(string userAccount);
        void RemoveChannel(string channelAccount);

        void SendVerificationCode(string mailto, string account, string code);
    }
}
