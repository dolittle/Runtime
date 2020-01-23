// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ProcessingResult" /> where processing failed.
    /// </summary>
    public class FailedProcessingResult : ProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedProcessingResult"/> class.
        /// </summary>
        public FailedProcessingResult()
            : base(ProcessingState.Failed)
        {
        }
    }
}