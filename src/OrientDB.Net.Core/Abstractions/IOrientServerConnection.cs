﻿using OrientDB.Net.Core.Models;
using System.Collections.Generic;

namespace OrientDB.Net.Core.Abstractions
{
    public interface IOrientServerConnection
    {
        IOrientDatabaseConnection CreateDatabase(string database, DatabaseType databaseType, StorageType type);
        IOrientDatabaseConnection DatabaseConnect(string database, DatabaseType storageType, int poolSize = 1);
        //IOrientDatabaseConnection Handshake(string database);
        void DeleteDatabase(string database, StorageType storageType);
        bool DatabaseExists(string database, StorageType storageType);
        void Shutdown(string username, string password);
        IEnumerable<string> ListDatabases();
        string GetConfigValue(string name);
        void SetConfigValue(string name, string value);
    }
}
