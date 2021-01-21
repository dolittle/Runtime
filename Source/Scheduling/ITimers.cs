// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Scheduling
{
    /// <summary>
    /// Defines a system for working with <see cref="Timer">timers</see>.
    /// </summary>
    public interface ITimers
    {
        /// <summary>
        /// Perform an action on every given interval in milliseconds.
        /// </summary>
        /// <param name="interval">Interval in milliseconds.</param>
        /// <param name="action"><see cref="Action"/> to perform.</param>
        /// <returns>A <see cref="ITimer"/>.</returns>
        ITimer Every(double interval, Action action);
    }
}