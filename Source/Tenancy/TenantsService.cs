// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Tenancy.Contracts.Tenants;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="TenantsBase"/>.
    /// </summary>
    public class TenantsService : TenantsBase
    {
        readonly ITenants _tenants;
        readonly ILogger _logger;
        
        public TenantsService(ITenants tenants, ILogger logger)
        {
            _tenants = tenants;
            _logger = logger;
        }

        /// <inheritdoc />
        public override Task<Contracts.GetAllResponse> GetAll(Contracts.GetAllRequest request, ServerCallContext context)
        {
            try
            {
                _logger.GetAll();
                var response = new Contracts.GetAllResponse();
                
                response.Tenants.AddRange(_tenants.All.Select(ToProtobuf));
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Failure(ex);
                return Task.FromResult(new Contracts.GetAllResponse { Failure = ex.ToProtobuf() });
            }
        }

        static Contracts.Tenant ToProtobuf(TenantId tenantId)
            => new()
            {
                Id = tenantId.ToProtobuf()
            };
    }
}
