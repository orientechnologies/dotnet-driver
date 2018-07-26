using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using OrientDB.Net.Core.Models;
using System.IO;

namespace Operations
{
    internal class DatabaseExistsOperation : IOrientDBOperation<DatabaseExistsResult>
    {
        private readonly string _database;
        private readonly ConnectionMetaData _metaData;
        private readonly ServerConnectionOptions _options;
        private readonly StorageType _storageType;
        private readonly byte[] token;

        public DatabaseExistsOperation(string database, StorageType storageType, ConnectionMetaData metaData, ServerConnectionOptions options)
        {
            _database = database;
            _metaData = metaData;
            _options = options;
            _storageType = storageType;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Synchronous, sessionId);
            ///standard request fields, token is mandatory from version 37
            request.AddDataItem((byte)OperationType.DB_EXIST);
            request.AddDataItem(request.SessionId);
            request.AddDataItem((byte[])token);


            if (DriverConstants.ProtocolVersion > 36)
            {
                request.AddDataItem(_database);
                request.AddDataItem(_storageType.ToString().ToLower());
            }

            //starting from version 37 token session is mandatory
            if (DriverConstants.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
            {

            }

            //since 1.5 snapshot but not in 1.5
            //since version 37 not in use
            if (_metaData.ProtocolVersion >= 16 && _metaData.ProtocolVersion <= 36) 
                request.AddDataItem(_storageType.ToString().ToLower());

            return request;
        }

        internal byte[] ReadToken(BinaryReader reader)
        {
            byte[] localToken = null;
            var size = reader.ReadInt32EndianAware();
            if (size > -1)
            {
                localToken = reader.ReadBytesRequired(size);
            };


            // Will need to set this.
            // if token renewed
            // if (token.Length > 0)
            //    _database.GetConnection().Token = token;

            return localToken;
        }

        public DatabaseExistsResult Execute(BinaryReader reader)
        {
            if (_metaData.ProtocolVersion > 26 && _metaData.UseTokenBasedSession)
                ReadToken(reader);

            // operation specific fields
            byte requestId = reader.ReadByte();
            //TODO do control check above
            byte existByte = reader.ReadByte();

            if (existByte == 0)
            {
                return new DatabaseExistsResult(false);
            }
            else
            {
                return new DatabaseExistsResult(true);
            }
        }
    }
}
