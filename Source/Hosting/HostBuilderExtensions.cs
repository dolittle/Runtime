// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Hosting.Scopes;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Adds a scoped host to this <see cref="IHostBuilder"/> that allows registering instances of <see cref="IHostedService"/> that are executed in an isolated DI container.
    /// The services added using the configure callback will only be available to the hosted services in this scoped host.
    /// </summary>
    /// <remarks>
    /// Configuration, Properties and the <see cref="HostBuilderContext"/> will be shared with the parent host builder.
    /// </remarks>
    /// <param name="builder">The parent host builder to add the scoped hosted services to.</param>
    /// <param name="configure">The callback to use to configure the scoped host.</param>
    /// <returns>The parent host builder for continuation.</returns>
    public static IHostBuilder AddScopedHost(this IHostBuilder builder, Action<IHostBuilder> configure)
    {
        var scopedBuilder = new ScopedHostBuilder(builder);
        configure(scopedBuilder);
        return builder;
    }
}
