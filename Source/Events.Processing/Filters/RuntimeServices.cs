// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly FiltersService _filtersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="filtersService">The <see cref="FiltersService"/>.</param>
        public RuntimeServices(FiltersService filtersService)
        {
            _filtersService = filtersService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Events.Processing";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new Service[]
            {
                new Service(_filtersService, Contracts.Filters.BindService(_filtersService), Contracts.Filters.Descriptor)
            };
    }
}