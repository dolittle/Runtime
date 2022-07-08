// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Defines a system that can commit external events.
/// </summary>
public interface ICommitExternalEvents
{
    /// <summary>
    /// Commits external events to a scoped event log.
    /// </summary>
    /// <param name="events">The <see cref="CommittedEvents"/> external events to commit.</param>
    /// <param name="consent">The <see cref="ConsentId"/>.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    /// <returns>A <see cref="Task"/> that is resolved when external event is committed.</returns>
    Task Commit(CommittedEvents events, ConsentId consent, ScopeId scope);
}
