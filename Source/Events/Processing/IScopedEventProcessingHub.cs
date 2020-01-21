// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the hub for working with <see cref="ScopedEventProcessor">scoped event processors</see>.
    /// </summary>
    /// <remarks>
    /// Tenant aware centalized Hub for processing events within the bounded context.
    /// </remarks>
    public interface IScopedEventProcessingHub
    {
        /// <summary>
        /// Register a <see cref="ScopedEventProcessor"/>.
        /// </summary>
        /// <param name="processor"><see cref="ScopedEventProcessor"/> to register.</param>
        void Register(ScopedEventProcessor processor);

        /// <summary>
        /// Process a <see cref="CommittedEventStream"/>.
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEventStream"/> to process.</param>
        void Process(CommittedEventStream committedEventStream);

        /// <summary>
        /// Being processing events.
        /// </summary>
        void BeginProcessingEvents();
    }
}