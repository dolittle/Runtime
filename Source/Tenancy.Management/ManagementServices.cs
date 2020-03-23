// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.Tenancy.Management;

namespace Dolittle.Runtime.Tenancy.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> for exposing
    /// management service implementations for Heads.
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly TenantsService _tenantsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementServices"/> class.
        /// </summary>
        /// <param name="tenantsService"><see cref="TenantsService"/> to expose.</param>
        public ManagementServices(TenantsService tenantsService)
        {
            _tenantsService = tenantsService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Tenancy";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_tenantsService, grpc.Tenants.BindService(_tenantsService), grpc.Tenants.Descriptor)
            };
        }
    }
}