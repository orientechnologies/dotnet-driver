using System;
using System.Collections.Generic;
using System.Text;

namespace OrientDB.Net.Serializers.NetworkBinary
{

    public class BytesContainer
    {

        public byte[] bytes;
        public int offset;

        public BytesContainer(byte[] data)
        {
            bytes = data;
        }

        public BytesContainer()
        {
            bytes = new byte[64];
        }

        public BytesContainer(byte[] data, int iOffset)
        {
            this.bytes = data;
            this.offset = iOffset;
        }

        public BytesContainer copy()
        {
            return new BytesContainer(bytes, offset);
        }

        public int alloc(int toAlloc)
        {
            int cur = offset;
            offset += toAlloc;
            if (bytes.Length < offset)
                resize();
            return cur;
        }

        public int allocExact(int toAlloc)
        {
            int cur = offset;
            offset += toAlloc;
            if (bytes.Length < offset)
            {
                byte[] newArray = new byte[offset];
                Array.Copy(bytes, 0, newArray, 0, bytes.Length);
                bytes = newArray;
            }
            return cur;
        }

        public BytesContainer skip(int read)
        {
            offset += read;
            return this;
        }

        public byte[] fitBytes()
        {
            if (bytes.Length == offset)
            {
                return bytes;
            }
            byte[] fitted = new byte[offset];
            Array.Copy(bytes, 0, fitted, 0, offset);
            return fitted;
        }

        private void resize()
        {
            int newLength = bytes.Length;
            while (newLength < offset)
                newLength *= 2;
            byte[] newBytes = new byte[newLength];
            Array.Copy(bytes, 0, newBytes, 0, bytes.Length);
            bytes = newBytes;
        }

    }
}
