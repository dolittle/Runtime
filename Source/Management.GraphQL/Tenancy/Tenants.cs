// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Management.GraphQL.Tenancy
{
    public class Tenants
    {
        readonly ITenants _tenants;

        public Tenants(ITenants tenants)
        {
            _tenants = tenants;
        }

        public IEnumerable<Guid> All() => _tenants.All.Select(_ => _.Value);
    }
}