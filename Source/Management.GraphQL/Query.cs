// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Management.GraphQL
{
    public class Query
    {
        public EventHandlers.EventHandlers GetEventHandlers() => new();

        public Tenancy.Tenants GetTenants() => new object() as Tenancy.Tenants;
    }
}