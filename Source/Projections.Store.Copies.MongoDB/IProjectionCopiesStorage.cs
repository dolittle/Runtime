// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Defines a system that can provide an <see cref="IMongoDatabase"/> to store Projection read model copies in.
/// </summary>
public interface IProjectionCopiesStorage
{
    /// <summary>
    /// Gets the <see cref="IMongoDatabase"/> to store Projection read model copies in.
    /// </summary>
    IMongoDatabase Database { get; }
}
