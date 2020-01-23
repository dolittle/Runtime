// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a base representation of <see cref="IProcessingResult" />.
    /// </summary>
    public class ProcessingResult : IProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingResult"/> class.
        /// </summary>
        /// <param name="state">The <see cref="ProcessingState"/>.</param>
        public ProcessingResult(ProcessingState state) => Value = state;

        /// <inheritdoc/>
        public ProcessingState Value { get; }
    }
}