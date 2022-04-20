// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Persistence;

/// <summary>
/// Defines a system that can write <see cref="Commit">commits</see> to persistent storage.
/// </summary>
public interface IPersistCommits
{
    /// <summary>
    /// Persists a <see cref="Commit"/> to persistent storage.
    /// </summary>
    /// <param name="commit">The commit to persist.</param>
    /// <param name="cancellationToken">A cancellation token to use to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the result of the operation.</returns>
    Task<Try> Persist(Commit commit, CancellationToken cancellationToken);
}
