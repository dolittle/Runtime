// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the processing result for when processing should be tried again.
    /// </summary>
    public interface IRetryProcessingResult : IProcessingResult
    {
        /// <summary>
        /// Gets the amount milliseconds from now for when next retry should occur.
        /// </summary>
        uint RetryTimeout { get; }
    }
}