// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Actors;

public class Services : ICanAddServices
{
    public void AddTo(IServiceCollection services)
        => services.AddSingleton<ICreateProps>(_ => new CreateProps(_));
}
