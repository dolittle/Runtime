// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Lifecycle;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB
{
    /// <summary>
    /// Represents a connection to the MongoDB Projections database.
    /// </summary>
    [SingletonPerTenant]
    public class ProjectionsConnection : IProjectionsConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreConnection"/> class with default scoped collections.
        /// </summary>
        /// <param name="connection">A connection to the MongoDB database.</param>
        protected ProjectionsConnection(DatabaseConnection connection)
        {
            MongoClient = connection.MongoClient;
            Database = connection.Database;
        }

        /// <summary>
        /// Gets the <see cref="IMongoClient"/> configured for the MongoDB database.
        /// </summary>
        protected IMongoClient MongoClient { get; }

        /// <summary>
        /// Gets the <see cref="IMongoDatabase" />.
        /// </summary>
        protected IMongoDatabase Database { get; }

        /// <summary>
        /// Starts a client session.
        /// </summary>
        /// <param name="options">The <see cref="ClientSessionOptions" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IClientSessionHandle" />.</returns>
        public IClientSessionHandle StartSession(ClientSessionOptions options = default, CancellationToken cancellationToken = default) => MongoClient.StartSession(options, cancellationToken);

        /// <summary>
        /// Starts a client session.
        /// </summary>
        /// <param name="options">The <see cref="ClientSessionOptions" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="IClientSessionHandle" />.</returns>
        public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = default, CancellationToken cancellationToken = default) => MongoClient.StartSessionAsync(options, cancellationToken);
    }
}
