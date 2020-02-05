// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" /> interacting with a <see cref="RemoteFilterProcessor" />.
    /// </summary>
    public interface IFilterHub
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="targetStreamId">The <see cref="StreamId" /> that represents the new stream.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> that represents the stream that this filter filtering on.</param>
        void Register(StreamId targetStreamId, StreamId sourceStreamId);
    }
}