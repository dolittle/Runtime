// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Exception that gets thrown when the event received from the event horizon is not at the expected public stream position.
    /// </summary>
    public class WrongEventReceivedOverEventHorizons : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongEventReceivedOverEventHorizons" /> class.
        /// </summary>
        /// <param name="expectedPosition">The expected public stream position.</param>
        /// <param name="publicStreamPosition">The public stream position of the received event.</param>
        public WrongEventReceivedOverEventHorizons(StreamPosition expectedPosition, StreamPosition publicStreamPosition)
            : base($"Received event from event horizon with public stream position {publicStreamPosition.Value} but expected it to be at position {expectedPosition.Value}")
        {
        }
    }
}