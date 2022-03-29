using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace NullLib.TocReborn
{
    public class TocEmptyMessage : TocMessage
    {
        [JsonIgnore]
        public override TocMessageKind MessageKind => TocMessageKind.Empty;
    }
}
