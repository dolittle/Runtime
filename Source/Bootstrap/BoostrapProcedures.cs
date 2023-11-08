// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Bootstrap;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Server.Bootstrap;

/// <summary>
/// Represents an implementation of <see cref="IBootstrapProcedures"/>.
/// </summary>
[Singleton]
public class BoostrapProcedures : IBootstrapProcedures
{
    readonly IEnumerable<ICanPerformBoostrapProcedure> _procedures;
    readonly TenantId[] _tenants;
    bool _performedBootstrap;

    public BoostrapProcedures(IEnumerable<ICanPerformBoostrapProcedure> procedures, ITenants tenants)
    {
        _procedures = procedures;
        _tenants = tenants.All.ToArray();
    }

    /// <inheritdoc />
    public async Task PerformAll()
    {
        if (_performedBootstrap)
        {
            throw new BootstrapProceduresAlreadyPerformed();
        }

        foreach (var procedure in _procedures.OrderByDescending(it => it.Priority))
        {
            await procedure.Perform();
            await Task.WhenAll(_tenants.Select(procedure.PerformForTenant));
        }

        _performedBootstrap = true;
    }
}
