// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ProcessingResult" /> where processing succeeded.
    /// </summary>
    public class SucceededProcessingResult : ProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SucceededProcessingResult"/> class.
        /// </summary>
        public SucceededProcessingResult()
            : base(ProcessingState.Succeeded)
        {
        }
    }
}