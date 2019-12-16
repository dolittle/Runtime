// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that knows about <see cref="IEventProcessor">event processors</see>.
    /// </summary>
    public interface IEventProcessors
    {
        /// <summary>
        /// Gets all the available <see cref="IEventProcessor">event processors</see>.
        /// </summary>
        IEnumerable<IEventProcessor> All { get; }
    }
}