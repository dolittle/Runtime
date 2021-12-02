// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Defines a system that knows about the connection string to a MongoDB resource for the current tenant.
/// </summary>
public interface IKnowTheConnectionString
{
    /// <summary>
    /// Gets the <see cref="MongoUrl">connection string</see> for the MongoDB resource for the current tenant.
    /// </summary>
    MongoUrl ConnectionString { get; }
}