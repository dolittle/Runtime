// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents a tenant.
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// </summary>
        /// <param name="id"><see cref="TenantId">Identifier</see>.</param>
        public Tenant(TenantId id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the <see cref="TenantId"/>.
        /// </summary>
        public TenantId Id { get; }
    }
}