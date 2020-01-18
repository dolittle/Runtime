// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Represents the result of the processing of an event.
    /// </summary>
    public class ProcessingResult
    {
        /// <summary>
        /// Gets or sets the <see cref="StreamState">state</see>.
        /// </summary>
        public StreamState StreamState { get; set; }
    }
}