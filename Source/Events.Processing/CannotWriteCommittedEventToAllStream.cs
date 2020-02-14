// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Exception that gets thrown when <see cref="IWriteEventsToStreams" /> is attempting to write a committed event to the all stream.
    /// </summary>
    public class CannotWriteCommittedEventToAllStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotWriteCommittedEventToAllStream"/> class.
        /// </summary>
        public CannotWriteCommittedEventToAllStream()
            : base("Cannot write an already committed event to the all stream")
        {
        }
    }
}