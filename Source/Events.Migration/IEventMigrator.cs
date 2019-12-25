// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Defines the functionality for a migrator that migrates from an older generation of <see cref="IEvent">Event</see> to a newer generation.
    /// </summary>
    /// <typeparam name="TIn">Older generation of the <see cref="IEvent">Event</see> to migrate from.</typeparam>
    /// <typeparam name="TOut">Newer generation of the <see cref="IEvent">Event</see> to migrate to.</typeparam>
    /// <remarks>
    /// Types inheriting from this interface will be automatically registered.
    /// </remarks>
    public interface IEventMigrator<in TIn, out TOut>
        where TIn : IEvent
        where TOut : IEvent
    {
        /// <summary>
        /// Migrates from the incoming <see cref="IEvent">Event</see> to the outgoing <see cref="IEvent">Event</see>.
        /// </summary>
        /// <param name="source">Older version of the <see cref="IEvent">Event</see>.</param>
        /// <returns>Newer version of the <see cref="IEvent">Event</see>.</returns>
        TOut Migrate(TIn source);
    }
}
