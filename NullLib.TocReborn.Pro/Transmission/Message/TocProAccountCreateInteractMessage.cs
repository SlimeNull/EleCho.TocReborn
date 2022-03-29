using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullLib.TocReborn.Pro
{
    public class TocProAccountCreateInteractMessage : TocProInteractMessage
    {
        public override TocProMessageKind ProMessageKind => TocProMessageKind.AccountCreateInteract;

        public TocProAccountCreateInteractMessage()
        {
        }

        public TocProAccountCreateInteractMessage(string account)
        {
            Account = account;
        }

        public TocProAccountCreateInteractMessage(TocProMessageResponse? response) => Response = response;

        [ResponseField]
        public string? Account { get; set; }
        public string? EMail { get; set; }

        public const int ErrorCannotCreate = -1;
        public const int ErrorEmptyMailAddr = -2;
        public const int ErrorCannotSendCode = -3;
    }
}
