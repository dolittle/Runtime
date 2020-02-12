// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolittle.ResourceTypes.Configuration;
using Dolittle.Runtime.Events.MongoDB;
using Mongo2Go;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class a_mongo_db_connection : IDisposable
    {
        internal MongoDbRunner _runner;

        internal string _databaseName = Guid.NewGuid().ToString();
        internal ConcurrentDictionary<Type, object> _collections = new ConcurrentDictionary<Type, object>();

        public a_mongo_db_connection()
        {
            _runner = MongoDbRunner.Start(additionalMongodArguments: "--quiet --syslog");
            var configurationForMock = new Mock<IConfigurationFor<EventStoreConfiguration>>();
            configurationForMock.Setup(_ => _.Instance).Returns(new EventStoreConfiguration
            {
                ConnectionString = _runner.ConnectionString,
                Database = _databaseName
            });

            Connection = new Connection(configurationForMock.Object);
        }

        public Connection Connection { get; }

        public static IList<T> ReadBsonFile<T>(string fileName)
        {
            string[] content = File.ReadAllLines(fileName);
            return content.Select(s => BsonSerializer.Deserialize<T>(s)).ToList();
        }

        public void Dispose()
        {
            Connection.Database.Client.DropDatabase(_databaseName);
            _runner.Dispose();
        }

        internal IMongoCollection<T> GetCollection<T>()
        {
            return _collections.GetOrAdd(typeof(T), Connection.Database.GetCollection<T>(typeof(T).Name)) as IMongoCollection<T>;
        }
    }
}