// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Implementation of <see cref="ITenantOffsetRepository" />.
    /// </summary>
    public class TenantOffsetRepository : ITenantOffsetRepository
    {
        readonly FactoryFor<IGeodesics> _getGeodesics;
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantOffsetRepository"/> class.
        /// </summary>
        /// <param name="getGeodesics">a <see cref="FactoryFor{IGeodesics}" /> for retrieving the correctly scoped geodesics.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager" /> for ensuring the correct execution context.</param>
        public TenantOffsetRepository(FactoryFor<IGeodesics> getGeodesics, IExecutionContextManager executionContextManager)
        {
            _getGeodesics = getGeodesics;
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc />
        public IEnumerable<TenantOffset> Get(IEnumerable<TenantId> tenants, EventHorizonKey key)
        {
            var offsets = new List<TenantOffset>();
            Parallel.ForEach(tenants, (tenant) =>
            {
                _executionContextManager.CurrentFor(tenant);
                using (var geodesics = _getGeodesics())
                {
                    var offset = geodesics.GetOffset(key);
                    offsets.Add(new TenantOffset(tenant, offset));
                }
            });
            return offsets;
        }
    }
}