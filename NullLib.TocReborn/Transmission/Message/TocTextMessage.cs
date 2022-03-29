using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NullLib.TocReborn
{
    public class TocTextMessage : TocMessage
    {
        public override TocMessageKind MessageKind => TocMessageKind.Text;
        public string? Text { get;set; }

        public TocTextMessage(string? text) => Text = text;

        public TocTextMessage()
        {
        }
    }
}
