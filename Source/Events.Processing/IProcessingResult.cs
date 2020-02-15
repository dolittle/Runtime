// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the processing result.
    /// </summary>
    public interface IProcessingResult
    {
        /// <summary>
        /// Gets a value indicating whether processing succeeded.
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        /// Gets a value indicating whether to retry processing.
        /// </summary>
        bool Retry { get; }

        /// <summary>
        /// Gets the reason for failure.
        /// </summary>
        string FailureReason { get; }
    }
}