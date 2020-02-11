// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents a null implementation of <see cref="NullEventToStreamsWriter"/>.
    /// </summary>
    public class NullEventToStreamsWriter : IWriteEventsToStreams
    {
        /// <inheritdoc/>
        public Task<bool> Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId)
        {
            return Task.FromResult(true);
        }
    }
}