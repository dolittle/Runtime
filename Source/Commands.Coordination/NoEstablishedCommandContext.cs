// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Exception that gets thrown when one is trying to perform operations and there is no <see cref="ICommandContext"/> established.
    /// </summary>
    public class NoEstablishedCommandContext : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEstablishedCommandContext"/> class.
        /// </summary>
        public NoEstablishedCommandContext()
            : base("There is no established command context")
        {
        }
    }
}