// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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
        /// Gets the reason for why processing failed.
        /// </summary>
        string FailureReason { get; }

        /// <summary>
        /// Gets a value indicating whether to retry processing.
        /// </summary>
        bool Retry { get; }

        /// <summary>
        /// Gets the retry timeout <see cref="TimeSpan" />.
        /// </summary>
        TimeSpan RetryTimeout { get; }
    }
}