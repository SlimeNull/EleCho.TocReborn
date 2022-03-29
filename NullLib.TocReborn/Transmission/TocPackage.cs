using System;
using System.IO;

namespace NullLib.TocReborn
{
    public class TocPackage
    {
        public TocPackage(TocPackageHeader header, byte[] content)
        {
            Header = header;
            Content = content;
        }

        public TocPackage(TocPackageKind packageKind, TocMessageKind messageKind, byte[] content) :
            this(new TocPackageHeader(packageKind, messageKind, (uint)content.LongLength), content) { }

        public TocPackage(TocPackageKind packageKind, TocMessageKind messageKind, Guid guid, byte[] content) :
            this(new TocPackageHeader(packageKind, messageKind, (uint)content.LongLength, guid), content)
        { }


        public TocPackageHeader Header { get; set; }
        public byte[] Content { get; set; }

        public static TocPackage CreatePing(bool isResponse)
        {
            return isResponse ?
                new TocPackage(TocPackageKind.Ping, TocMessageKind.Empty, new byte[0]) :
                new TocPackage(TocPackageKind.Ping, TocMessageKind.Empty, new byte[1]);
        }
    }
}
