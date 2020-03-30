// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the failure types of the processing of an event.
    /// </summary>
    public enum ProcessorFailureType
    {
        /// <summary>The unknown type.</summary>
        Unknown = 0,

        /// <summary>The transient type.</summary>
        Transient = 1,

        /// <summary>The persistent type.</summary>
        Persistent = 2
    }
}