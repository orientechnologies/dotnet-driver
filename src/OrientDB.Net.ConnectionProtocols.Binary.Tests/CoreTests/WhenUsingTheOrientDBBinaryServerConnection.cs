﻿using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OrientDB.Net.ConnectionProtocols.Binary.Core;
using OrientDB.Net.Core.Abstractions;
using OrientDB.Net.Core.Models;
using System;
using System.Linq;
using System.Reflection;

namespace OrientDB.Net.ConnectionProtocols.Binary.Tests.CoreTests
{
    [TestFixture]
    public class WhenUsingTheOrientDBBinaryServerConnection
    {
        [Test]
        public void ItShouldThrowAnExceptionWhenPassedNullOptions()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new OrientDBBinaryServerConnection(null, null, null));
        }

        [Test]
        public void ItShouldThrowAnExceptionWhenPassedANUllSerializer()
        {
            var options = new DatabaseConnectionOptions
            {
                HostName = "localhost",
                Password = "000",
                Port = 2424,
                UserName = "root",
            };

            Assert.Throws(typeof(ArgumentNullException), () => new OrientDBBinaryServerConnection(options, null, null));
        }

        [Test]
        public void ItShouldThrowAnExceptionWhenCreatingADatabaseWithANullOrZeroLengthDatabaseName()
        {
            var options = new DatabaseConnectionOptions
            {
                HostName = "localhost",
                Password = "000",
                Port = 2424,
                UserName = "root",

            };
            Mock<IOrientDBRecordSerializer<byte[]>> mockSerializer = new Mock<IOrientDBRecordSerializer<byte[]>>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            var conn = new OrientDBBinaryServerConnection(options, mockSerializer.Object, mockLogger.Object);

            Assert.Throws(typeof(ArgumentException), () => conn.CreateDatabase(null, DatabaseType.Document, StorageType.Memory));
        }

        [Test]
        public void ItShouldThrowAnExceptionWhenDroppingADatabaseWithANullOrZeroLengthDatabaseName()
        {
            var options = new DatabaseConnectionOptions
            {
                HostName = "localhost",
                Password = "000",
                Port = 2424,
                UserName = "root",

            };
            Mock<IOrientDBRecordSerializer<byte[]>> mockSerializer = new Mock<IOrientDBRecordSerializer<byte[]>>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            var conn = new OrientDBBinaryServerConnection(options, mockSerializer.Object, mockLogger.Object);

            Assert.Throws(typeof(ArgumentException), () => conn.DeleteDatabase(null, StorageType.Memory));
        }

        [Test]
        public void ItShouldThrowAnExceptionWhenCallingDatabaseExistsWithANullOrZeroLengthDatabaseName()
        {
            var options = new DatabaseConnectionOptions
            {
                HostName = "localhost",
                Password = "000",
                Port = 2424,
                UserName = "root",

            };
            Mock<IOrientDBRecordSerializer<byte[]>> mockSerializer = new Mock<IOrientDBRecordSerializer<byte[]>>();
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            var conn = new OrientDBBinaryServerConnection(options, mockSerializer.Object, mockLogger.Object);

            Assert.Throws(typeof(ArgumentException), () => conn.DatabaseExists(null, StorageType.PLocal));
        }

        // Testing this is incredibly difficult. I will need to review the structure and
        // make modifications.
        public void ItShouldSuccessfullyExecuteDatabaseExistsWhithCorrectArguments()
        {
            Mock<OrientDBBinaryServerConnection> mockConnection = new Mock<OrientDBBinaryServerConnection>();

            var obj = mockConnection.Object;
            mockConnection.Setup(conn => conn.Open()).Callback(() =>
            {
                Mock<OrientDBNetworkConnectionStream> mockStream = new Mock<OrientDBNetworkConnectionStream>();
                //mockStream.Setup(stream => stream.Se)

                var objStr = mockStream.Object;
                var streamPoolProp = objStr.GetType().GetProperty("StreamPool", BindingFlags.NonPublic | BindingFlags.Instance);
                streamPoolProp.SetValue(objStr, Enumerable.Empty<object>());

                var prop = obj.GetType().GetField("_connectionStream", BindingFlags.NonPublic | BindingFlags.Instance);
                prop.SetValue(obj, mockStream);
            });

            obj.Open();

            mockConnection.VerifyAll();
        }
    }
}
