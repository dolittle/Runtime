// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Persistence;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Defines a system that can update the high watermark offset for an event log.
/// </summary>
public interface IEventLogOffsetStore
{
    /// <summary>
    /// Updates the next offset for a given event log.
    /// </summary>
    /// <param name="session">The <see cref="IClientSessionHandle"/> for the MongoDB transaction.</param>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="nextOffset">The next offset to write to this eventlog</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the result of the operation.</returns>
    Task UpdateOffset(IClientSessionHandle session, ScopeId scopeId, ulong nextOffset, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets the next offset for a given event log.
    /// </summary>
    /// <param name="scopeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ulong> GetNextOffset(ScopeId scopeId, CancellationToken cancellationToken);
}
