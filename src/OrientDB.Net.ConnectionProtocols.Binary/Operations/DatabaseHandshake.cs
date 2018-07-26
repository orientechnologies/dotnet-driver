using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using System;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseHandshake : IOrientDBOperation<DatabaseHandshakeResult>
    {
        private readonly ConnectionMetaData _connectionMetaData;
        private readonly ServerConnectionOptions _options;

        public DatabaseHandshake(ServerConnectionOptions _options, ConnectionMetaData connectionMetaData)
        {
            this._options = _options;
            this._connectionMetaData = connectionMetaData;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Asynchronous);

            //from version 37 handshake is required
            //TODO HANDSHAKE should support reconnect and connection drop
            request.AddDataItem((byte)OperationType.HANDSHAKE);

            // operation specific fields
            if (DriverConstants.ProtocolVersion > 36)
            {
                request.AddDataItem(DriverConstants.ProtocolVersion);
                request.AddDataItem(DriverConstants.DriverName);
                request.AddDataItem(DriverConstants.DriverVersion);
                request.AddDataItem((byte)EncodingType.ENCODING_DEFAULT);
                request.AddDataItem((byte)EncodingType.ERROR_MESSAGE_STRING);
            }

            return request;
        }

        //Not best solution because handshake should not return any response
        public DatabaseHandshakeResult Execute(BinaryReader reader)
        {
            return new DatabaseHandshakeResult(true);
        }
    }
}

