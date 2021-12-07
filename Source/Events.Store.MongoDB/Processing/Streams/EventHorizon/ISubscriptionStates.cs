// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;

/// <summary>
/// Defines a system that knows about the Subscription State repositories.
/// </summary>
public interface ISubscriptionStates : IEventStoreConnection
{
    /// <summary>
    /// Gets a Subscription collection.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> of <see cref="AbstractStreamProcessorState" />.</returns>
    Task<IMongoCollection<MongoDB.Processing.Streams.EventHorizon.SubscriptionState>> Get(ScopeId scopeId, CancellationToken token);
}