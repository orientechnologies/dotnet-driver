using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using System;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using System.IO;
using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class ServerOpenOperation : IOrientDBOperation<OpenServerResult>
    {
        private readonly ConnectionMetaData _connectionMetaData;
        private readonly ServerConnectionOptions _options;
        private readonly byte[] _connectionToken;

        public ServerOpenOperation(ServerConnectionOptions _options, ConnectionMetaData connectionMetaData)
        {
            this._options = _options;
            this._connectionMetaData = connectionMetaData;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Synchronous);

            // standard request fields
            request.AddDataItem((byte)OperationType.CONNECT);
            request.AddDataItem(request.SessionId);
            request.AddDataItem((new byte[0]));

            // operation specific fields



            //if(DriverConstants.ProtocolVersion >36)
            //{
            //    //use token
            //    request.AddDataItem((byte)(_connectionMetaData.UseTokenBasedSession ? 1 : 1));

            //}

            

            request.AddDataItem(_options.UserName);
            request.AddDataItem(_options.Password);

            return request;
        }

        public OpenServerResult Execute(BinaryReader reader)
         {
            

            OpenServerResult result = new OpenServerResult();


            
            var size = reader.ReadInt32EndianAware();
            if (size > 0)
            {
                var read = reader.ReadBytes(size);

            }    
            
            var RequestType = reader.ReadByte();
            var sessionId = reader.ReadInt32EndianAware();
            result.SessionId = sessionId;

            size = reader.ReadInt32EndianAware();
            if (size > 0)
            {
                var token = reader.ReadBytes(size);
                result.Token = token;
            }

            

            if (_connectionMetaData.ProtocolVersion > 26)
            {
                
                
               
            }

            return result;
        }
    }
}
