// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Tenancy.Management;
using Dolittle.Protobuf;
using Dolittle.Runtime.Management;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Tenancy.Management.Tenants;
using grpc = contracts::Dolittle.Runtime.Tenancy.Management;

namespace Dolittle.Runtime.Tenancy.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="TenantsBase"/>.
    /// </summary>
    public class TenantsService : TenantsBase
    {
        readonly ITenants _tenants;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantsService"/> class.
        /// </summary>
        /// <param name="tenants">Underlying <see cref="ITenants"/> system.</param>
        public TenantsService(ITenants tenants)
        {
            _tenants = tenants;
        }

        /// <inheritdoc/>
        public override async Task GetTenants(TenantsRequest request, IServerStreamWriter<TenantsResponse> responseStream, ServerCallContext context)
        {
            await _tenants.All.Forward(
                responseStream,
                context,
                _ => _.Tenants,
                _ => new grpc.Tenant
                {
                    Id = _.ToProtobuf()
                }).ConfigureAwait(false);
        }
    }
}