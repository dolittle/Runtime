// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a system that can schedule callbacks to be called at regular intervals.
    /// </summary>
    public interface ICanScheduleCallbacks
    {
        /// <summary>
        ///  Registers the given callback to be called at every interval, starting immediately. The callback can be 
        ///  cancelled by disposing the returned <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="callback">The <see cref="Action"/> to call immediately and at each interval afterwards.</param>
        /// <param name="interval">A <see cref="TimeSpan"/> for the interval between each call.</param>
        /// <returns>A <see cref="IDisposable"/>, which when disposed will cancel the callback.</returns>
        IDisposable ScheduleCallback(Action callback, TimeSpan interval);
    }
}
