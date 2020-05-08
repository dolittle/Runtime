// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="StreamEvent"/> has a different <see cref="PartitionId"/> as expected.
    /// </summary>
    public class ExpectedStreamEventWithSameGivenPartition : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedStreamEventWithSameGivenPartition"/> class.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent"/> with the wrong <see cref="PartitionId"/>.</param>
        /// <param name="givenPartition">The expected <see cref="PartitionId"/>.</param>
        public ExpectedStreamEventWithSameGivenPartition(StreamEvent @event, PartitionId givenPartition)
            : base($"Expected StreamEvent {@event} to have the same PartitionId as {givenPartition}")
        {
        }
    }
}
