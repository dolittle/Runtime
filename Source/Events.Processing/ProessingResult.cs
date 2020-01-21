// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the state of the processing of an event.
    /// </summary>
    public enum ProcessingResult
    {
        /// <summary>The state when processing succeeded.</summary>
        Succeeded = 0,

        /// <summary>The state when processing failed.</summary>
        Failed,

        /// <summary>The result when processing failed and it should retry processing.</summary>
        Retry
    }
}