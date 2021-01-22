// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Time
{
    /// <summary>
    /// Defines a clock that keeps track of the current system date and time.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Retrieves the current system date and time.
        /// </summary>
        /// <returns>The current system <see cref="DateTimeOffset">date and time</see>.</returns>
        DateTimeOffset GetCurrentTime();
    }
}
