// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Scheduling
{
    /// <summary>
    /// Represents an implementation of <see cref="ITimers"/>.
    /// </summary>
    public class Timers : ITimers
    {
        /// <inheritdoc/>
        public ITimer Every(double interval, Action action)
        {
            var timer = new Timer(action, interval);
            return timer;
        }
    }
}