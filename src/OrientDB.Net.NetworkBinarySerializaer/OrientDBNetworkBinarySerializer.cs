using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using OrientDB.Net.Serializers.NetworkBinary.Extensions;
using System.Collections;
using System.Globalization;
using OrientDB.Net.Serializers.NetworkBinary.Models;
using System.IO;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Serializers.NetworkBinary;

namespace OrientDB.Net.Serializers.NetworkBinary
{
    public class OrientDBNetworkBinarySerializer : IOrientDBRecordSerializer<byte[]>
    {

        public OrientDBRecordFormat RecordFormat
        {
            get
            {
                return OrientDBRecordFormat.Binary;
            }
        }

        public OrientDBNetworkBinarySerializer()
        {

        }

        /// <summary>
        /// Generic type function for deserialization.
        /// </summary>
        /// <typeparam name="TResultType"></typeparam>
        /// <param name="data"></param>
        /// <returns>"TResultType"</returns>
        public TResultType Deserialize<TResultType>(byte[] data) where TResultType : OrientDBEntity
        {
            TResultType entity = Activator.CreateInstance<TResultType>();

            DictionaryOrientDBEntity document = new DictionaryOrientDBEntity();

            var stream = new MemoryStream(data);
            var reader = new BinaryReader(stream);
            
            document.OClassName = BinaryReaderHelper.ReadString(reader);
            var FieldNumber = BinaryReaderHelper.ReadSignedVarLong(reader);
            for (int i = 0; i < FieldNumber; i++)
            {
                var FieldName = BinaryReaderHelper.ReadString(reader);
                var FieldType = (OrientType)reader.ReadByte();
                var FieldValue = DeserializeValue(reader, FieldType);
                document.SetField(FieldName, FieldValue);
            }

            return (TResultType)Convert.ChangeType(document, typeof(TResultType));

        }

        //Deserializing object value depending on type from OrientType enum
        Object DeserializeValue(BinaryReader reader, OrientType type)
        {
            switch (type)
            {
                case OrientType.Integer:
                    return BinaryReaderHelper.ReadAsInteger(reader);
                case OrientType.Long:
                    return BinaryReaderHelper.ReadAsLong(reader);
                case OrientType.Short:
                    return BinaryReaderHelper.ReadAsShort(reader);
                case OrientType.String:
                    return BinaryReaderHelper.ReadString(reader);
                case OrientType.Double:
                    return BitConverter.Int64BitsToDouble(BinaryReaderHelper.ReadAsLong(reader));
                case OrientType.Float:
                    return BinaryReaderHelper.ReadFloat(reader);
                case OrientType.Decimal:
                    return BinaryReaderHelper.ReadDecimal(reader);
                case OrientType.Byte:
                    return reader.ReadByte();
                case OrientType.EmbeddedList:
                    break;
            }
            return null;
        }



        public byte[] Serialize<T>(T input) where T : OrientDBEntity
        {
            return Encoding.UTF8.GetBytes($"{input.OClassName}@{SerializeEntity(input)}");
        }




    }
}
