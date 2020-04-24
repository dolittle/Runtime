// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="IPerformActionOnAllTenants" />.
    /// </summary>
    public class ActionOnAllTenantsPerformer : IPerformActionOnAllTenants
    {
        readonly ITenants _tenants;
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionOnAllTenantsPerformer"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        public ActionOnAllTenantsPerformer(ITenants tenants, IExecutionContextManager executionContextManager)
        {
            _tenants = tenants;
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc/>
        public void Perform(Action<TenantId> action)
        {
            var originalExecutionContext = _executionContextManager.Current;
            try
            {
                foreach (var tenant in _tenants.All.ToArray())
                {
                    _executionContextManager.CurrentFor(tenant);
                    action(tenant);
                }
            }
            finally
            {
                _executionContextManager.CurrentFor(originalExecutionContext);
            }
        }
    }
}