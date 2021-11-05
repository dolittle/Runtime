// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly AggregateRootsService _aggregateRoots;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="aggregateRoots">The <see cref="AggregateRootsService"/>.</param>
        public RuntimeServices(AggregateRootsService aggregateRoots)
        {
            _aggregateRoots = aggregateRoots;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Aggregates";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new[]
            {
                new Service(_aggregateRoots, Contracts.AggregateRoots.BindService(_aggregateRoots), Contracts.AggregateRoots.Descriptor)
            };
    }
}
