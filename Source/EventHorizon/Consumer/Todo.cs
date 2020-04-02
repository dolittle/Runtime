// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when we're missing something in DotNET.Fundamentals.
    /// </summary>
    public class Todo : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Todo"/> class.
        /// </summary>
        public Todo()
            : base($"Please do me")
        {
        }
    }
}