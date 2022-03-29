using System;
using System.Collections.Generic;
using System.Text;

namespace NullLib.TocReborn
{
    public class PackageEventArgs
    {
        public TocPackage? Package { get; }

        public PackageEventArgs(TocPackage? package) => Package = package;
    }
}