// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.Server.Bootstrap;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public class DatabaseMigrator : ICanPerformBoostrapProcedure
{
    readonly Func<TenantId, IDbMigration> _getMigrator;

    public DatabaseMigrator(Func<TenantId, IDbMigration> getMigrator)
    {
        _getMigrator = getMigrator;
    }

    public async Task PerformForTenant(TenantId tenant)
    {
        await _getMigrator(tenant).MigrateTenant();
    }


    public int Priority => 1000;
}
