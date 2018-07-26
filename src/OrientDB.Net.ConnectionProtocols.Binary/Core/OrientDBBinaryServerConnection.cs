﻿using Operations;
using OrientDB.Net.ConnectionProtocols.Binary.Operations;
using System;
using OrientDB.Net.Core.Abstractions;
using System.Collections.Generic;
using OrientDB.Net.Core.Models;
using Microsoft.Extensions.Logging;
using OrientDB.Net.ConnectionProtocols.Binary.Operations.Results;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class OrientDBBinaryServerConnection : IOrientServerConnection
    {
        private readonly ServerConnectionOptions _options;
        private readonly IOrientDBRecordSerializer<byte[]> _serializer;
        private OrientDBNetworkConnectionStream _connectionStream;
        private bool _databaseHanshakeResult;
        private readonly ILogger _logger;
        private readonly DatabaseHandshakeResult databaseHandshakeResult;


        public OrientDBBinaryServerConnection(ServerConnectionOptions options, IOrientDBRecordSerializer<byte[]> serializer, ILogger logger)
        {
            _logger = logger;

            _logger.LogDebug("OrientDBBinaryServerConnection.Ctor()");
            _options = options ?? throw new ArgumentNullException($"{nameof(options)} cannot be null.");
            _serializer = serializer ?? throw new ArgumentNullException($"{nameof(serializer)} cannot be null.");

            Open();
        }

        public void Handshake()
        {
            if (_databaseHanshakeResult == false)
            {
                _logger.LogDebug("Opening connections");
                foreach (var stream in _connectionStream.StreamPool)
                {
                    _connectionStream.Send(new DatabaseHandshake(_options, _connectionStream.ConnectionMetaData));
                }
                _databaseHanshakeResult = true;
            }

            Open();

        }

        public void Open()
        {
            if (_databaseHanshakeResult == false)
            {
                _connectionStream = new OrientDBNetworkConnectionStream(_options, _logger);
                _logger.LogDebug("Opening connections");
                foreach (var stream in _connectionStream.StreamPool)
                {
                    var _openResult = _connectionStream.Send(new DatabaseHandshake(_options, _connectionStream.ConnectionMetaData));//Sending handshake here, only for testing purpose

                }
            }

            _logger.LogDebug("Opening connections");
            foreach (var stream in _connectionStream.StreamPool)
            {
                var _openResult = _connectionStream.Send(new ServerOpenOperation(_options, _connectionStream.ConnectionMetaData));
                stream.SessionId = _openResult.SessionId;
                stream.Token = _openResult.Token;
            }

        }

        public void Dispose()
        {
            _connectionStream.Close();
        }

        public IOrientDatabaseConnection CreateDatabase(string database, DatabaseType databaseType, StorageType storageType)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException($"{nameof(database)} cannot be null or zero length.");

            _logger.LogDebug($"Creating database {database}. DatabaseType: {databaseType}. StorageType: {storageType}.");
            return _connectionStream.Send(new DatabaseCreateOperation(database, databaseType, storageType,
                _connectionStream.ConnectionMetaData, _options, _serializer, _logger, _connectionStream));
        }

        public IOrientDatabaseConnection DatabaseConnect(string database, DatabaseType type, int poolSize = 1)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException($"{nameof(database)} cannot be null or zero length.");

            _logger.LogDebug($"Connecting to database: {database}. DatabaseType: {type}. Pool Size: {poolSize}");
            return new OrientDBBinaryConnection(new DatabaseConnectionOptions
            {
                Database = database,
                HostName = _options.HostName,
                Password = _options.Password,
                PoolSize = poolSize,
                Port = _options.Port,
                Type = type,
                UserName = _options.UserName
            }, _serializer, _logger, _connectionStream);

        }

        public void DeleteDatabase(string database, StorageType storageType)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException($"{nameof(database)} cannot be null or zero length.");

            _logger.LogDebug($"Deleting database {database}. StorageType: {storageType}.");
            _connectionStream.Send(new DatabaseDropOperation(database, storageType, _connectionStream.ConnectionMetaData, _options));
        }

        public bool DatabaseExists(string database, StorageType storageType)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException($"{nameof(database)} cannot be null or zero length.");

            return _connectionStream.Send(new DatabaseExistsOperation(database, storageType, _connectionStream.ConnectionMetaData, _options)).Exists;
        }

        public void Shutdown(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"{nameof(username)} cannot be null.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"{nameof(password)} cannot be null.");

            _connectionStream.Send(new ServerShutdownOperation(_connectionStream.ConnectionMetaData, username, password));
        }

        public IEnumerable<string> ListDatabases()
        {
            throw new NotImplementedException();
        }

        public string GetConfigValue(string name)
        {
            throw new NotImplementedException();
        }

        public void SetConfigValue(string name, string value)
        {
            throw new NotImplementedException();
        }
    }
}
