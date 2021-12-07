// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB;

/// <summary>
/// Represents a resource configuration for a MongoDB Embedding implementation.
/// </summary>
public class EmbeddingsConfiguration
{
    /// <summary>
    /// Gets or sets the MongoDB servers.
    /// </summary>
    public IEnumerable<string> Servers { get; set; }

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the maximum connection pool size for the MongoDB client.
    /// </summary>
    public int MaxConnectionPoolSize { get; set; } = MongoDefaults.MaxConnectionPoolSize;
}