// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Booting.Stages;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Extensions for building <see cref="InitialSystemSettings"/>.
/// </summary>
public static class BasicsBootBuilderExtensions
{
    /// <summary>
    /// Set scheduling to be synchronous.
    /// </summary>
    /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
    /// <param name="assembly"><see cref="Assembly"/> to use as entry assembly.</param>
    /// <returns>Chained <see cref="BootBuilder"/>.</returns>
    /// <remarks>
    /// If an entry assembly is not specified, the system will use the <see cref="Assembly.GetEntryAssembly()"/> method.
    /// </remarks>
    public static IBootBuilder WithEntryAssembly(this IBootBuilder bootBuilder, Assembly assembly)
    {
        bootBuilder.Set<BasicsSettings>(_ => _.EntryAssembly, assembly);
        return bootBuilder;
    }

    /// <summary>
    /// Set <see cref="Environment"/> to development - default is production.
    /// </summary>
    /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
    /// <returns>Chained <see cref="BootBuilder"/>.</returns>
    public static IBootBuilder Development(this IBootBuilder bootBuilder)
    {
        bootBuilder.Set<BasicsSettings>(_ => _.Environment, Environment.Development);
        return bootBuilder;
    }
}