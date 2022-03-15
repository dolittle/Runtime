// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Server.HealthChecks
{
    public class Bindings : ICanProvideBindings
    {
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IConfigureOptions<HealthCheckOptions>>().To<EndpointConfiguration>();
            builder.Bind<IConfigureOptions<HealthCheckServiceOptions>>().To<HealthCheckConfiguration>();
        }
    }
}

