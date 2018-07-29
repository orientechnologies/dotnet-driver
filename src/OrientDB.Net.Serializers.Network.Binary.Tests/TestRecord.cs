
using Moq;
using NUnit.Framework;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.Core.Abstractions;

using Microsoft.Extensions.Logging;

using OrientDB.Net.Core.Models;

using System.Linq;
using System.Reflection;
using OrientDB.Net.SqlCommandBuilder.Protocol;
using OrientDB.Net.SqlCommandBuilder.Extensions;

namespace OrientDB.Net.Serializers.Network.Binary.Tests
{
    class TestRecord
    {
        // common parameteres 
        private string HostName = "localhost";
        private string Password = "000";
        private int Port = 2424;
        private string UserName = "root";


        [Test]
        public void TestLoadNoFetchPlan()
        {
            QueryCompiler store = new QueryCompiler();
            var options1 = new DatabaseConnectionOptions
            {
                HostName = HostName ,
                Password = Password,
                Port = Port,
                UserName = UserName,

            };
        
            var options2 = new DatabaseConnectionOptions
            {
                HostName = HostName,
                Password = Password,
                Port = Port,
                UserName = UserName,
                Database = "ConnectionTest",
                PoolSize = 1,

            };
            Mock<IOrientDBRecordSerializer<byte[]>> mockSerializer = new Mock<IOrientDBRecordSerializer<byte[]>>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            ServerConnectionOptions serverConnectionOptions = new ServerConnectionOptions();
            OrientDBNetworkConnectionStream connection = new OrientDBNetworkConnectionStream(serverConnectionOptions, mockLogger.Object);

            var testContext = new OrientDBBinaryServerConnection(options1, mockSerializer.Object, mockLogger.Object);

            var database = new OrientDBBinaryConnection(options2, mockSerializer.Object, mockLogger.Object, connection);

            
            database.ExecuteCommand("CREATE CLASS Person");



            DictionaryOrientDBEntity document = new DictionaryOrientDBEntity();
            document.SetField("foo", "foo string value");
            document.SetField("bar", 12345);

            OrientDBEntity obj = null;
            if (obj is OrientDBEntity)
            {
                document = (obj as OrientDBEntity).ToDictionaryOrientDBEntity();
            }
            else
            {
                document = OrientDBEntityExtensions.ToDictionaryOrientDBEntity(obj);
            }



            //DictionaryOrientDBEntity insertedDocument;
            //insertedDocument.Hydrate();
            //insertedDocument.Into("TestClass");
            //insertedDocument.Run();


            //var loaded = database.Load.ORID(insertedDocument.ORID).Run();
            Assert.AreEqual(document.OClassName, "TestClass");
            Assert.AreEqual(document.GetField<string>("foo"), document.GetField<string>("foo"));
            Assert.AreEqual(document.GetField<int>("bar"), document.GetField<int>("bar"));
            //Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
        }
    }
}


