// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Exception that gets thrown when the ping interval is not greater than zero milliesecond.
    /// </summary>
    public class PingIntervalNotGreaterThanZero : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PingIntervalNotGreaterThanZero"/> class.
        /// </summary>
        public PingIntervalNotGreaterThanZero()
            : base("The ping interval must be greater than zero milliseconds")
        {
        }
    }
}
