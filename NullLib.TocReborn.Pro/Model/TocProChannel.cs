#if NET47
#endif

namespace NullLib.TocReborn.Pro
{
    /// <summary>
    /// TOC 频道
    /// </summary>
    public class TocProChannel : TocProUserBase
    {
        public string? OwnerAccount { get; set; }
        public string[]? Members { get; set; }
    }
}
