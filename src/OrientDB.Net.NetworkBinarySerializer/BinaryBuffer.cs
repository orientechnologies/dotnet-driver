using OrientDB.Net.Core.Models;
using OrientDB.Net.Serializers.NetworkBinary.Extensions;
using System;
using System.Collections.Generic;

namespace OrientDB.Net.Serializers.NetworkBinary
{
    internal class BinaryBuffer : List<Byte>
    {
        public int Length { get { return Count; } }

        public byte[] bytes;
        public int offset;

        internal int Allocate(int allocationSize)
        {
            var currentPosition = Length;
            AddRange(new Byte[allocationSize]);
            return currentPosition;
        }

        internal void Write(int offset, int value)
        {
            var rawValue = BinarySerializer.ToArray(value);
            for (int i = 0; i < rawValue.Length; i++)
            {
                this[offset + i] = rawValue[i];
            }
        }

       

        //internal String stringFromBytes(byte[] data, int offset, int len)
        //{
        //    try
        //    {
        //        return new String(data, offset, len);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        internal void Write(int offset, byte value)
        {
            this[offset] = value;
        }

        internal int WriteVariant(long value)
        {
            var pos = Length;
            var unsignedValue = signedToUnsigned(value);
            writeUnsignedVarLong(unsignedValue);
            return pos;
        }

        private void writeUnsignedVarLong(ulong value)
        {
            while ((value & 0xFFFFFFFFFFFFFF80L) != 0L)
            {
                Add((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }
            Add((byte)(value & 0x7F));
        }

        private int readAsInteger(int offset, byte[] value)
        {
            return (int)readSignedVarLong(offset,value);
        }

        private  long readSignedVarLong(int offset, byte[] value)
        {
            long raw = readUnsignedVarLong(offset, value);
            // This undoes the trick in writeSignedVarLong()
            long temp = (((raw << 63) >> 63) ^ raw) >> 1;
            // This extra step lets us deal with the largest signed values by
            // treating
            // negative results from read unsigned methods as like unsigned values
            // Must re-flip the top bit if the original read value had it set.
            return temp ^ (raw & (1L << 63));
        }

        private long readUnsignedVarLong(int offset, byte[] value)
        {
            long returnValue = 0L;
            int i = 0;
            long b;
            while ((b = value[offset++] & 0x80L) != 0)
            {
                returnValue |= (b & 0x7F) << i;
                i += 7;
                if (i > 63)
                {
                    //thr
                }
                    //throw new IllegalArgumentException("Variable length quantity is too long (must be <= 63)");
            }
            return returnValue | (b << i);
        }

        private uint signedToUnsigned(int value)
        {
            return (uint)((value << 1) ^ (value >> 31));
        }

        private ulong signedToUnsigned(long value)
        {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        internal int Write(string value)
        {
            var pos = Length;
            WriteVariant(value.Length);
            AddRange(BinarySerializer.ToArray(value));
            return pos;
        }

        internal int Write(double value)
        {
            var pos = Length;
            AddRange(BitConverter.GetBytes(value).CheckEndianess());
            return pos;
        }

        internal int Write(float value)
        {
            var pos = Length;
            AddRange(BitConverter.GetBytes(value).CheckEndianess());
            return pos;
        }

        internal int Write(byte value)
        {
            var pos = Length;
            Add(value);
            return pos;
        }

        internal int Write(ORID rid)
        {
            var pos = Length;
            WriteVariant(rid.ClusterId);
            WriteVariant(rid.ClusterPosition);
            return pos;
        }

    }
}