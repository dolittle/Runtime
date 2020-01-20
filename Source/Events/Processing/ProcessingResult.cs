// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// The result of a processing.
    /// </summary>
    public enum ProcessingResult
    {
        /// <summary>Ok.</summary>
        Ok = 0,

        /// <summary>Failure.</summary>
        Failure,

        /// <summary>Retry.</summary>
        Retry,

        /// <summary>Ignore.</summary>
        Ignore
    }
}