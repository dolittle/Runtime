// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Defines a system for working with <see cref="TenantId">tenants</see>.
    /// </summary>
    public interface ITenants
    {
        /// <summary>
        /// Gets all available tenants represented by their <see cref="TenantId"/>.
        /// </summary>
        ObservableCollection<TenantId> All { get; }
    }
}