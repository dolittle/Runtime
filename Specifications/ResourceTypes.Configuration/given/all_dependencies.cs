// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.ResourceTypes.Configuration.Specs.given
{
    public class all_dependencies
    {
        public static readonly ResourceType first_resource_type = "first_resource_type";
        public static readonly ResourceType second_resource_type = "second_resource_type";
        public static readonly ResourceType third_resource_type = "third_resource_type";
        public static readonly ResourceTypeImplementation first_resource_type_implementation = "first_resource_type_implementation";
        public static readonly ResourceTypeImplementation second_resource_type_implementation = "second_resource_type_implementation";
        protected static readonly TenantId tenant_id = TenantId.System;
    }
}