// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates;

/// <summary>
/// Defines a system knows about the <see cref="IMongoCollection{TDocument}" /> for <see cref="AggregateRoot" />.
/// </summary>
public interface IAggregatesCollection : IEventStoreConnection
{
    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}" /> of <see cref="AggregateRoot" />.
    /// </summary>
    IMongoCollection<AggregateRoot> Aggregates { get; }
}