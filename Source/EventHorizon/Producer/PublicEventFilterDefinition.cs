// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="WellKnownStreamFilterDefinition" /> for the public events stream.
    /// </summary>
    public class PublicEventFilterDefinition : WellKnownStreamFilterDefinition
    {
        /// <summary>
        /// The name of the public events stream.
        /// </summary>
        public const string PublicEventsStreamName = "PublicEvents";

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilterDefinition"/> class.
        /// </summary>
        public PublicEventFilterDefinition()
            : base(StreamId.AllStreamId, StreamId.PublicEventsId, PublicEventsStreamName)
        {
        }
    }
}