//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace OrientDB.Net.Serializers.Network.Binary.Tests
//{
//    [TestFixture]
//    public class RecordDeserialize
//    {
//        [Test]
//        public void ShouldCreateAndDeleteDatabase()
//        {
//            string databaseName = "thisIsTestDatabaseForNetDriver";
//            OServer server = TestConnection.GetServer();

//            bool exists = server.DatabaseExist(databaseName, OStorageType.PLocal);

//            if (exists)
//            {
//                server.DropDatabase(databaseName, OStorageType.PLocal);

//                exists = server.DatabaseExist(databaseName, OStorageType.PLocal);
//            }

//            Assert.AreEqual(exists, false);

//            if (!exists)
//            {
//                bool isCreated = server.CreateDatabase(databaseName, ODatabaseType.Graph, OStorageType.PLocal);

//                Assert.AreEqual(isCreated, true);

//                if (isCreated)
//                {
//                    server.DropDatabase(databaseName, OStorageType.PLocal);

//                    exists = server.DatabaseExist(databaseName, OStorageType.PLocal);

//                    Assert.AreEqual(exists, false);
//                }
//            }
//        }

//    }
//}

