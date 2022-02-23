// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB;

/// <summary>
/// Defines a connection to the MongoDB database.
/// </summary>
public interface IDatabaseConnection
{
    /// <summary>
    /// Gets the configured <see cref="IMongoClient"/> for the MongoDB database.
    /// </summary>
    public IMongoClient MongoClient { get; }

    /// <summary>
    /// Gets the configured <see cref="IMongoDatabase"/> for the MongoDB database.
    /// </summary>
    public IMongoDatabase Database { get; }
}
