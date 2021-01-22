// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Time
{
    /// <summary>
    /// Represents an implementation of <see cref="ISystemClock"/>.
    /// </summary>
    public class SystemClock : ISystemClock
    {
        /// <summary>
        /// Retrieves the current system date and time.
        /// </summary>
        /// <returns>The current system <see cref="DateTime">date and time</see>.</returns>
        public DateTimeOffset GetCurrentTime()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}