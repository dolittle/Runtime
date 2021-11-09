// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly TenantsService _tenantsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="tenantsService">The <see cref="TenantsService"/>.</param>
        public RuntimeServices(TenantsService tenantsService)
        {
            _tenantsService = tenantsService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Tenancy";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new Service[]
            {
                new Service(_tenantsService, Contracts.Tenants.BindService(_tenantsService), Contracts.Tenants.Descriptor)
            };
    }
}
