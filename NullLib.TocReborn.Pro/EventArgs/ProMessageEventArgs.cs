using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullLib.TocReborn.Pro
{
    public class ProMessageEventArgs : EventArgs
    {
        public TocProMessage? Message { get; set; }

        public ProMessageEventArgs(TocProMessage? message) => Message = message;
    }
}
