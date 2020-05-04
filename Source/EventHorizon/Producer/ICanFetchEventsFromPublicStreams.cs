// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Defines a system that can fetch events from public event streams.
    /// </summary>
    public interface ICanFetchEventsFromPublicStreams : ICanFetchEventsFromPartitionedStream
    {
    }
}
