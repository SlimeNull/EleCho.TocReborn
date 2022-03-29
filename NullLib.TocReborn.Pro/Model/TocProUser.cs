
namespace NullLib.TocReborn.Pro
{
    /// <summary>
    /// TOC 用户信息 (账户或频道)
    /// </summary>
    public abstract class TocProUserBase
    {
        public string? Name { get; set; }
        public string? Account { get; set; }
        public string? AvatarUri { get; set; }
        public string? Description { get; set; }
    }
}
