// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Booting.Stages;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Extensions for building <see cref="ContainerSettings"/>.
    /// </summary>
    public static class ContainerSettingsBootBuilderExtensions
    {
        /// <summary>
        /// Define which container to be used during application lifecycle.
        /// </summary>
        /// <param name="bootBuilder">The <see cref="IBootBuilder"/>.</param>
        /// <typeparam name="T"><see cref="Type"/> of <see cref="IContainer">container</see>.</typeparam>
        /// <returns>Chained <see cref="Bootloader"/> for configuration.</returns>
        /// <remarks>
        /// This is normally discovered using the interface <see cref="ICanProvideContainer"/>
        /// but in some cases you might need to be explicit, e.g. when you have a wrapper around
        /// the actual container.
        /// </remarks>
        public static IBootBuilder UseContainer<T>(this IBootBuilder bootBuilder)
            where T : IContainer
        {
            bootBuilder.Set<ContainerSettings>(_ => _.ContainerType, typeof(T));
            return bootBuilder;
        }
    }
}