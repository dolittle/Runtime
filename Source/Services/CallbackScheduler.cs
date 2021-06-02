// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents a <see cref="ICanScheduleCallbacks"/>.
    /// </summary>
    public class CallbackScheduler : ICanScheduleCallbacks
    {

        /// <inheritdoc/>
        public IDisposable RegisterCallback(Action callback, TimeSpan delay)
        {
            return new Timer((object state) => callback(), null, TimeSpan.Zero, delay);
        }
    }
}
