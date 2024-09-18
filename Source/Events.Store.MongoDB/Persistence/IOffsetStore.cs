// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Defines a system that can update the high watermark offset for an event log.
/// </summary>
public interface IOffsetStore
{
    /// <summary>
    /// Updates the next offset for a given event log / stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="session">The <see cref="IClientSessionHandle"/> for the MongoDB transaction.</param>
    /// <param name="writtenOffset">The offset that was just written</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the result of the operation.</returns>
    Task UpdateOffset(string stream, IClientSessionHandle session, ulong writtenOffset,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the next offset for a given stream
    /// </summary>
    /// <param name="stream">The stream to get the next offset for</param>
    /// <param name="session">The <see cref="IClientSessionHandle"/> for the MongoDB transaction.(Optional)</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task<ulong> GetNextOffset(string stream, IClientSessionHandle? session, CancellationToken cancellationToken);
}
