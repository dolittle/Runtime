// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.ResourceTypes.Configuration.Specs.given
{
    public class resource_type_with_first_service_for_first_resource_type_and_first_implementation : IRepresentAResourceType
    {
        IDictionary<Type, Type> _bindings;

        public ResourceType Type => all_dependencies.first_resource_type;

        public ResourceTypeImplementation ImplementationName => all_dependencies.first_resource_type_implementation;

        public Type ConfigurationObjectType => typeof(configuration_for_first_resource_type);

        public IDictionary<Type, Type> Bindings
        {
            get
            {
                if (_bindings == null)
                    InitializeBindings();

                return _bindings;
            }
        }

        void InitializeBindings()
        {
            _bindings = new Dictionary<Type, Type>
            {
                { typeof(first_service), typeof(implementation_of_first_service_for_first_implementation_type) }
            };
        }
    }
}