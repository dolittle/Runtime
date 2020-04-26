// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines the basis for the state of a <see cref="StreamProcessor" />.
    /// </summary>
    public interface IStreamProcessorState
    {
        /// <summary>
        /// Gets or sets the <see cref="StreamPosition" />.
        /// </summary>
        StreamPosition Position { get; set; }
    }
}
