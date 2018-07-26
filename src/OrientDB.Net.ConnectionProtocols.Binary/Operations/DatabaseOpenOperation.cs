using OrientDB.Net.ConnectionProtocols.Binary.Constants;
using OrientDB.Net.ConnectionProtocols.Binary.Contracts;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.ConnectionProtocols.Binary.Extensions;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;
using System.Collections.Generic;
using System.IO;

namespace OrientDB.Net.ConnectionProtocols.Binary.Operations
{
    internal class DatabaseOpenOperation : IOrientDBOperation<OpenDatabaseResult>
    {
        private readonly DatabaseConnectionOptions _options;
        private readonly ConnectionMetaData _metaData;

        public DatabaseOpenOperation(DatabaseConnectionOptions options, ConnectionMetaData metaData)
        {
            _options = options;
            _metaData = metaData;
        }

        public Request CreateRequest(int sessionId, byte[] token)
        {
            Request request = new Request(OperationMode.Synchronous);

            // standard request fields
            request.AddDataItem((byte)OperationType.DB_OPEN);
            request.AddDataItem(sessionId);
            request.AddDataItem(token);
            // operation specific fields

            //from version 37 token is required
            if (DriverConstants.ProtocolVersion > 26 && _metaData.ProtocolVersion <=36)
            {
                request.AddDataItem((byte)(_metaData.UseTokenBasedSession ? 0 : 1));              
            }

            request.AddDataItem(_options.Database);
            request.AddDataItem(_options.UserName);
            request.AddDataItem(_options.Password);

            return request;
        }

        public OpenDatabaseResult Execute(BinaryReader reader)
        {
            byte[] token = null;
            var by = reader.ReadBytes(4);//
            var RequestType = reader.ReadByte();
            var sessionId = reader.ReadInt32EndianAware();
            
            if (_metaData.ProtocolVersion > 26)
            {
                var size = reader.ReadInt32EndianAware();
                token = reader.ReadBytesRequired(size);
            }

            if (_metaData.ProtocolVersion < 37)
            {

                int clusterCount = -1;

                if (_metaData.ProtocolVersion >= 7)
                    clusterCount = (int)reader.ReadInt16EndianAware();
                else
                    clusterCount = reader.ReadInt32EndianAware();

                List<Cluster> clusters = new List<Cluster>();

                if (clusterCount > 0)
                {
                    for (int i = 1; i <= clusterCount; i++)
                    {
                        Cluster cluster = new Cluster();

                        int clusterNameLength = reader.ReadInt32EndianAware();

                        byte[] clusterByte = reader.ReadBytes(clusterNameLength);
                        cluster.Name = System.Text.Encoding.UTF8.GetString(clusterByte, 0, clusterByte.Length);

                        cluster.Id = reader.ReadInt16EndianAware();

                        if (_metaData.ProtocolVersion < 24)
                        {
                            int clusterTypeLength = reader.ReadInt32EndianAware();

                            byte[] clusterTypeByte = reader.ReadBytes(clusterTypeLength);
                            string clusterType = System.Text.Encoding.UTF8.GetString(clusterTypeByte, 0, clusterTypeByte.Length);

                            if (_metaData.ProtocolVersion >= 12)
                                cluster.DataSegmentID = reader.ReadInt16EndianAware();
                            else
                                cluster.DataSegmentID = 0;
                        }
                        clusters.Add(cluster);
                    }
                }

                int clusterConfigLength = reader.ReadInt32EndianAware();

                byte[] clusterConfig = null;

                if (clusterConfigLength > 0)
                {
                    clusterConfig = reader.ReadBytes(clusterConfigLength);
                }

                string release = reader.ReadInt32PrefixedString();
            }

            return new OpenDatabaseResult(sessionId, token);
            
        }
    }
}
