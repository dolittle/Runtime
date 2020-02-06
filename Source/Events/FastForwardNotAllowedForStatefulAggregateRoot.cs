// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Exception that gets thrown when attempting to retrieve a statefull aggregate root without replaying events (fast-forwarding).
    /// </summary>
    public class FastForwardNotAllowedForStatefulAggregateRoot : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FastForwardNotAllowedForStatefulAggregateRoot"/> class.
        /// </summary>
        /// <param name="type"><see cref="AggregateRoot"/> type.</param>
        public FastForwardNotAllowedForStatefulAggregateRoot(Type type)
            : base($"Cannot fast forward stateful aggregate root of type '{type.AssemblyQualifiedName}'.")
        {
        }
    }
}