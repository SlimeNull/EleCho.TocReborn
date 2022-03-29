using NullLib.TocReborn;

namespace NullLib.TocReborn.Pro
{
    public enum TocProMessageKind : byte
    {
        Empty            = 50,
        Text             = 50 + 1,
        Image            = 50 + 2,
        File             = 50 + 3,

        Interact                 = 60,
        AccountCreateInteract    = 60 + 1,
        AccountVerifyInteract    = 60 + 2,
        AccountLoginInteract     = 60 + 3,
        AccountDeleteInteract    = 60 + 4,
        AccountModifyInteract    = 60 + 5,

        ChannelCreateInteract    = 70 + 1,
        ChannelDeleteInteract    = 70 + 2,
        ChannelJoinInteract      = 70 + 3,
        ChannelExitInteract      = 70 + 4,
        ChannelModifyInteract    = 70 + 5,
    }
}