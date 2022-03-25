// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;
using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Represents a resource configuration for a MongoDB Read model implementation.
/// </summary>
[TenantConfiguration("resources:readModels")]
public class MongoDBConfiguration
{
    /// <summary>
    /// Gets or sets the MongoDB host.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether or not to use SSL.
    /// </summary>
    public bool UseSSL { get; set; }

    /// <summary>
    /// Gets or sets the maximum connection pool size for the MongoDB client.
    /// </summary>
    public int MaxConnectionPoolSize { get; set; } = MongoDefaults.MaxConnectionPoolSize;
}
