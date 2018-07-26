//using OrientDB.Net.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace OrientDB.Net.Serializers.NetworkBinary
//{
//    internal class OVarIntSerializer
//    {

//        //public static short ReadAsShort(Bytes)
//        //{
//        //    return (short)

//        //}

//        //private long ReadSignedVarLong(bytes)
//        //{
//        //    long raw = ReadUnsignedVarLong(bytes);
//        //    // This undoes the trick in writeSignedVarLong()
//        //    long temp = (((raw << 63) >> 63) ^ raw) >> 1;
//        //    // This extra step lets us deal with the largest signed values by
//        //    // treating
//        //    // negative results from read unsigned methods as like unsigned values
//        //    // Must re-flip the top bit if the original read value had it set.
//        //    return temp ^ (raw & (1L << 63));
//        //}

//        //public readonly long ReadUnsignedVarLong(ORID bytes)
//        //{
//        //    long value = 0L;
//        //    int i = 0;
//        //    long b;
//        //    while (((b = bytes.(bytes.ClusterPosition++) & 0x80L) != 0)
//        //    {
//        //        value |= (b & 0x7F) << i;
//        //        i += 7;
//        //        if (i > 63)
//        //            throw new IllegalArgumentException("Variable length quantity is too long (must be <= 63)");
//        //    }
//        //    return value | (b << i);
//        //}


//    }

//    //public static short readAsShort(final BytesContainer bytes)
//    //{
//    //    return (short)readSignedVarLong(bytes);
//    //}

//    //public static long readAsLong(final BytesContainer bytes)
//    //{
//    //    return readSignedVarLong(bytes);
//    //}

//    //public static int readAsInteger(final BytesContainer bytes)
//    //{
//    //    return (int)readSignedVarLong(bytes);
//    //}




//}
