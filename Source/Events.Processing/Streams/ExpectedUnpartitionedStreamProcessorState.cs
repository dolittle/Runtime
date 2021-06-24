// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when an unpartitioned <see cref="IStreamProcessorState" /> was expected.
    /// </summary>
    public class ExpectedUnpartitionedStreamProcessorState : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedUnpartitionedStreamProcessorState"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        public ExpectedUnpartitionedStreamProcessorState(IStreamProcessorId streamProcessorId)
            : base($"Expected Stream Processor: '{streamProcessorId}' to be an unpartitioned stream processor")
        {
        }
    }
}