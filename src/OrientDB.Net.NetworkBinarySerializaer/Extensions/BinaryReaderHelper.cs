using OrientDB.Net.Core.Models;
using System;
using System.IO;

namespace OrientDB.Net.Serializers.NetworkBinary.Extensions
{
    internal static class BinaryReaderHelper
    {
        public static byte[] CheckEndianess(this byte[] b)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).CheckEndianess(), 0);
        }

        public static Int16 ReadInt16EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).CheckEndianess(), 0);
        }

        public static UInt32 ReadUInt32EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).CheckEndianess(), 0);
        }

        public static Int32 ReadInt32EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).CheckEndianess(), 0);
        }

        public static UInt64 ReadUInt64EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt64(binRdr.ReadBytesRequired(sizeof(UInt64)).CheckEndianess(), 0);
        }

        public static Int64 ReadInt64EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToInt64(binRdr.ReadBytesRequired(sizeof(Int64)).CheckEndianess(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }

        public static string ReadInt32PrefixedString(this BinaryReader binRdr)
        {
            int length = binRdr.ReadInt32EndianAware();

            if (length < 0)
                return null;

            byte[] int32Byte = binRdr.ReadBytes(length);
            return System.Text.Encoding.UTF8.GetString(int32Byte, 0, int32Byte.Length);
        }

        public static ORID ReadRid(this BinaryReader binRdr)
        {
            ORID orid = new ORID();
            orid.ClusterId = binRdr.ReadInt16EndianAware();
            orid.ClusterPosition = binRdr.ReadInt64EndianAware();
            return orid;
        }

        public static string ReadString(BinaryReader reader)
        {
            int len = ReadAsInteger(reader);
            byte[] rawBinary = reader.ReadBytesRequired(len);
            return System.Text.Encoding.UTF8.GetString(rawBinary, 0, rawBinary.Length);
        }

        public static int ReadAsInteger(BinaryReader reader)
        {
            return (int)ReadSignedVarLong(reader);
        }

        public static long ReadSignedVarLong(BinaryReader reader)
        {
            long raw = ReadUnsignedVarLong(reader);
            long temp = (((raw << 63) >> 63) ^ raw) >> 1;
            return temp ^ (raw & (1L << 63));
        }

        private static long ReadUnsignedVarLong(BinaryReader reader)
        {
            long value = 0L;
            int i = 0;
            long b = 0;
            while (((b = reader.ReadByte()) & 0x80) != 0)
            {
                value |= (b & 0x7F) << i;
                i += 7;
                if (i > 63)
                    throw new ArgumentOutOfRangeException("Variable length quantity is too long (must be <= 63)");

            }
            return value | (b << i);
        }

        public static short ReadAsShort(BinaryReader reader)
        {
            return (short)ReadSignedVarLong(reader);
        }

        public static long ReadAsLong(BinaryReader reader)
        {
            return ReadSignedVarLong(reader);
        }

        public static long ReadDate(BinaryReader reader)
        {
            return ReadSignedVarLong(reader);
        }

        public static float ReadFloat(BinaryReader reader)
        {
            return BitConverter.ToSingle(reader.ReadBytesRequired(sizeof(Int32)).CheckEndianess(), 0);
        }
        
        public static decimal ReadDecimal(BinaryReader reader)
        {
            var scale = reader.ReadInt32EndianAware();
            var valueSize = reader.ReadInt32EndianAware();
           // read Fine the value
            var valuex = reader.ReadBytesRequired(valueSize);

            Int64 x1 = 0;

            if ((valuex[0] & 0x80) == 0x80)
                x1 = (sbyte)valuex[0];
            else
                x1 = valuex[0];

            for (int i = 1; i < valuex.Length; i++)
            {
                x1 = (x1 << 8) | valuex[i];
            }
            return new Decimal(x1 * Math.Pow(10, (-1) * scale));
        }

    }
}

