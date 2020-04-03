// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when multiple filters are registered for the same <see cref="StreamId" />.
    /// </summary>
    public class CouldNotRegisterFilter : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotRegisterFilter"/> class.
        /// </summary>
        /// <param name="filter">The <see cref="IFilterDefinition" />.</param>
        /// <param name="reason">The <see cref="CouldNotRegisterFilterReason" />.</param>
        public CouldNotRegisterFilter(IFilterDefinition filter, CouldNotRegisterFilterReason reason)
            : base($"Could not register filter ({filter.SourceStream}, '{filter.TargetStream}'). {reason}")
        {
        }
    }
}
