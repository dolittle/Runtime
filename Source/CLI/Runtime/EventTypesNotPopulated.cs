// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime
{
    /// <summary>
    /// Exception that gets thrown when trying to get event types before they have been discovered.
    /// </summary>
    public class EventTypesNotPopulated : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesNotPopulated"/> class.
        /// </summary>
        public EventTypesNotPopulated()
            : base("Cannot get event types before it is populated")
        {}
    }
}
