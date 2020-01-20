// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the hub for working with <see cref="StreamProcessor">stream processors</see>.
    /// </summary>
    /// <remarks>
    /// Tenant aware centalized Hub for processing streams within the microservice.
    /// </remarks>
    public interface IStreamProcessingHub
    {
        /// <summary>
        /// Register a <see cref="StreamProcessor"/>.
        /// </summary>
        /// <param name="processor"><see cref="StreamProcessor"/> to register.</param>
        /// <param name="streamId"><see cref="StreamId"/> stream id.</param>
        void Register(StreamProcessor processor, StreamId streamId);

        /// <summary>
        /// Process a <see cref="CommittedEventStream"/>.
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEventStream"/> to process.</param>
        /// <returns><see cref="ProcessingResult">processing result</see>.</returns>
        ProcessingResult Process(CommittedEventStream committedEventStream);

        /// <summary>
        /// Being processing streams.
        /// </summary>
        void BeginProcessingStreams();
    }
}