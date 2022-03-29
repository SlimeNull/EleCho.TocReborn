using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullLib.TocReborn.Pro
{
    public class ProConnectionEventArgs : EventArgs
    {
        public TocProConnection? Connection { get; set; }

        public ProConnectionEventArgs(TocProConnection? conn) => Connection = conn;
    }
}
