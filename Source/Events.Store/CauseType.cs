// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the types of causes that can cause the event to occur.
    /// </summary>
    public enum CauseType
    {
        /// <summary>Indicates that the event was caused by a Command.</summary>
        Command = 1,

        /// <summary>Indicates that the event was caused by an event.</summary>
        Event = 2,
    }
}
