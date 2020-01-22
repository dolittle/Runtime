// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the types of causes that can cause <see cref="IEvent"/>s to occur.
    /// </summary>
    public enum CauseType
    {
        /// <summary>Indicates that the <see cref="IEvent"/> was caused by a Command.</summary>
        Command = 1,

        /// <summary>Indicates that the <see cref="IEvent"/> was caused by an <see cref="IEvent"/>.</summary>
        Event = 2,
    }
}
