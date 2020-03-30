// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IProcessingResult" />.
    /// </summary>
    public class ProcessingResult : IProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingResult"/> class.
        /// </summary>
        /// <param name="failure">The <see cref="ProcessorFailure" />.</param>
        public ProcessingResult(ProcessorFailure failure = null)
        {
            Failure = failure;
        }

        /// <inheritdoc />
        public bool Succeeded => Failure == null;

        /// <inheritdoc />
        public bool Retry => Failure?.Retry == true;

        #nullable enable
        /// <inheritdoc />
        public ProcessorFailure? Failure { get; }

        /// <summary>
        /// Implicitly converts the <see cref="EventHandlerClientToRuntimeResponse" /> to <see cref="ProcessingResult" />.
        /// </summary>
        /// <param name="response">The <see cref="EventHandlerClientToRuntimeResponse" />.</param>
        public static implicit operator ProcessingResult(EventHandlerClientToRuntimeResponse response) => new ProcessingResult(response.Failed);
    }
}