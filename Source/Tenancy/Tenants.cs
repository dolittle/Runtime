/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ITenants"/>
    /// </summary>
    [Singleton]
    public class Tenants : ITenants
    {
        bool _isInitialized;
        readonly Dictionary<TenantId, Tenant> _tenants = new Dictionary<TenantId, Tenant>();
        readonly ITenantsConfigurationManager _tenantsConfigurationManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantsConfigurationManager"></param>
        public Tenants(ITenantsConfigurationManager tenantsConfigurationManager)
        {
            _tenantsConfigurationManager = tenantsConfigurationManager;
            
            _tenants = tenantsConfigurationManager.Current.Tenants.ToDictionary(
                _ => _.Key,
                _ => new Tenant(_.Key)
            );
            RegisterDefaultTenants();
        }

        /// <inheritdoc/>
        public IEnumerable<TenantId> All => _tenants.Keys;

        void RegisterDefaultTenants()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _tenants[TenantId.Unknown] = new Tenant(TenantId.Unknown);
            _tenants[TenantId.System] = new Tenant(TenantId.System);
        }
    }   
}