// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Defines an event which is the subsequent generation of the <see cref="IEvent">Event</see>.
    /// </summary>
    /// <typeparam name="T">The previous generation of this event which this event supercedes.</typeparam>
    public interface IAmNextGenerationOf<T>
        where T : IEvent
    {
    }
}