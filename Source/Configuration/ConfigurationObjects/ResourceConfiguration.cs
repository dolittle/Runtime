// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

/// <summary>
/// Represents the resource configuration for a MongoDB resource.
/// </summary>
public class ResourceConfiguration
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
    /// Gets or sets the MongoDB servers.
    /// </summary>
    public string[] Servers { get; set; }

    /// <summary>
    /// Gets or sets the maximum connection pool size for the MongoDB client.
    /// </summary>
    public int MaxConnectionPoolSize { get; set; } = MongoDefaults.MaxConnectionPoolSize;

    public bool IsLegacy => !string.IsNullOrEmpty(Host);
}
