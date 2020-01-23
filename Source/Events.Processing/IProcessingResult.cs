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
        /// Gets the <see cref="ProcessingState" />.
        /// </summary>
        ProcessingState Value { get; }
    }
}