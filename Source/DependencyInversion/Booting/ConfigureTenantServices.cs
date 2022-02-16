// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.DependencyInversion.Booting;

public delegate void ConfigureTenantServices(TenantId tenantId, ContainerBuilder containerBuilder);
