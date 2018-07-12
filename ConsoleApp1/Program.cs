using OrientDB.Net.ConnectionProtocols.Binary;
using OrientDB.Net.Core;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using OrientDB.Net.Serializers.RecordCSVSerializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Person> persons = new List<Person>();

            Stopwatch swOrient = new Stopwatch();
            IOrientServerConnection server = new OrientDBConfiguration()
               .ConnectWith<byte[]>()
               .Connect(new BinaryProtocol("localhost", "root", "000"))
               .SerializeWith.Serializer(new OrientDBRecordCSVSerializer())
               .LogWith.Logger(new OrientDBLogger())
               .CreateFactory()
               .CreateConnection();
          

            IOrientDatabaseConnection database;

            if (server.DatabaseExists("ConnectionTest", StorageType.Local))
                database = server.DatabaseConnect("ConnectionTest", DatabaseType.Document);
            else
                database = server.CreateDatabase("ConnectionTest", DatabaseType.Document, StorageType.PLocal);

            database.ExecuteCommand("CREATE CLASS Person");

            var transaction = database.CreateTransaction();
            var person1 = new Person { Age = 33, FirstName = "Jane", LastName = "Doe", FavoriteColors = new[] { "black", "blue" } };
            transaction.AddEntity(person1);
            transaction.AddEntity(new Person { Age = 5, FirstName = "John", LastName = "Doe", FavoriteColors = new[] { "red", "blue" } });
            transaction.Commit();
            transaction = database.CreateTransaction();
            transaction.Remove(person1);
            transaction.Commit();

            persons = database.ExecuteQuery<Person>("SELECT * FROM Person");
        }
    }
}
