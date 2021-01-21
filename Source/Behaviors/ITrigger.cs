// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Behaviors
{
    /// <summary>
    /// Defines the basics mechanism of a trigger.
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// The event that gets called when the trigger triggers.
        /// </summary>
        event Triggered Triggered;
    }
}