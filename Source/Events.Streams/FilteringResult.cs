// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams.Processing;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents the result of the filtering of an event stream.
    /// </summary>
    public class FilteringResult : ProcessingResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the event should be included in the stream.
        /// </summary>
        public bool IncludeEvent { get; set; }

        /// <summary>
        /// Gets or sets the partition of an event in the stream.
        /// </summary>
        public int Partition { get; set; }
    }
}