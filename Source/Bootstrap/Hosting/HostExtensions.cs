// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Server.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Bootstrap.Hosting;

/// <summary>
/// Extensions for <see cref="IHost"/>.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Performs all bootstrap procedures.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> to perform the boot procedures for.</param>
    /// <returns>The builder for continuation.</returns>
    public static Task PerformBootstrap(this IHost host)
        => host.Services.GetRequiredService<IBootstrapProcedures>().PerformAll();
}
