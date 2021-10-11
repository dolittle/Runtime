// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store.MongoDB;
using Dolittle.Runtime.Events.Store.MongoDB;
using Dolittle.Runtime.ResourceTypes.Configuration;

namespace Dolittle.Runtime.Migrations.for_MigrationPerformer.given
{
    public class resources_builder
    {
        readonly Dictionary<Guid, ReadOnlyDictionary<string, dynamic>> _resources = new(); 
        
        public resources_builder configure_tenant(TenantId tenant, Action<resources_for_tenants_builder> callback)
        {
            var resources_for_tenants_builder = new resources_for_tenants_builder();
            callback(resources_for_tenants_builder);
            _resources.Add(tenant, resources_for_tenants_builder.build());
            return this;
        }

        public ResourceConfigurationsByTenant build()
            => new ResourceConfigurationsByTenant(new ReadOnlyDictionary<Guid, ReadOnlyDictionary<string, dynamic>>(_resources));

        public class resources_for_tenants_builder
        {
            readonly Dictionary<string, dynamic> _resources = new();

            public resources_for_tenants_builder with_event_store()
            {
                _resources.Add("eventStore", new EventStoreConfiguration());
                return this;
            }public resources_for_tenants_builder with_embeddings_store()
            {
                _resources.Add("embeddings", new EmbeddingsConfiguration());
                return this;
            }

            public ReadOnlyDictionary<string, dynamic> build() => new ReadOnlyDictionary<string, dynamic>(_resources);
        }
    }
}
