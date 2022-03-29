using System;
using System.Collections.Generic;
using System.Text;

namespace NullLib.TocReborn
{
    public struct TocPackageHeader
    {
        public const int HeaderByteLength = sizeof(TocPackageKind) + sizeof(TocMessageKind) + sizeof(uint) + GuidLength;
        private const int GuidLength = 16;

        public TocPackageHeader(TocPackageKind packageKind, TocMessageKind messageKind, uint messageLength)
        {
            PackageKind = packageKind;
            MessageKind = messageKind;
            MessageLength = messageLength;
            MessageGuid = Guid.NewGuid();
        }

        public TocPackageHeader(TocPackageKind packageKind, TocMessageKind messageKind, uint messageLength, Guid messageGuid)
        {
            PackageKind = packageKind;
            MessageKind = messageKind;
            MessageLength = messageLength;
            MessageGuid = messageGuid;
        }

        /// <summary>
        /// (byte) Kind of current Package
        /// </summary>
        public TocPackageKind PackageKind { get; set; }
        /// <summary>
        /// (byte) Kind of current Message
        /// </summary>
        public TocMessageKind MessageKind { get; set; }
        /// <summary>
        /// (uint) Payload Length
        /// </summary>
        public uint MessageLength { get; set; }
        /// <summary>
        /// (Guid) GUID of current package
        /// </summary>
        public Guid MessageGuid { get; set; }

        public static TocPackageHeader FromBytes(byte[] buffer, int index)
        {
            if (buffer == null || buffer.Length - index < HeaderByteLength)
                throw new ArgumentOutOfRangeException(nameof(buffer));
            ReadOnlySpan<byte> guidSpan = new ReadOnlySpan<byte>(buffer, index + 6, 16);
            return new TocPackageHeader()
            {
                PackageKind = (TocPackageKind)buffer[index],
                MessageKind = (TocMessageKind)buffer[index + 1],
                MessageLength = BitConverter.ToUInt32(buffer, index + 2),
                MessageGuid = new Guid(guidSpan),
            };
        }
        public static byte[] GetBytes(TocPackageHeader val)
        {
            byte[] buffer = new byte[HeaderByteLength];
            buffer[0] = (byte)val.PackageKind;
            buffer[1] = (byte)val.MessageKind;
            BitConverter.GetBytes(val.MessageLength).CopyTo(buffer, 2);
            val.MessageGuid.ToByteArray().CopyTo(buffer, 6);
            return buffer;
        }
    }
}
