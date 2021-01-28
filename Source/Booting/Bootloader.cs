// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents the starting point - the actual boot of an application with configuration options
    /// for what is possible to configure before actual booting.
    /// </summary>
    public class Bootloader
    {
        readonly Boot _boot;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootloader"/> class.
        /// </summary>
        /// <param name="boot"><see cref="Boot"/> to use.</param>
        public Bootloader(Boot boot) => _boot = boot;

        /// <summary>
        /// Configure boot.
        /// </summary>
        /// <param name="builderDelegate">Builder delegate.</param>
        /// <returns><see cref="Bootloader"/> for booting.</returns>
        public static Bootloader Configure(Action<IBootBuilder> builderDelegate)
        {
            var builder = new BootBuilder();
            builderDelegate(builder);
            var boot = builder.Build();
            return new Bootloader(boot);
        }

        /// <summary>
        /// Start booting.
        /// </summary>
        /// <returns><see cref="BootloaderResult">Result</see> from booting.</returns>
        public BootloaderResult Start()
        {
            var result = new BootStages().Perform(_boot);
            return new BootloaderResult(
                result.Container,
                result.GetAssociation<ITypeFinder>(WellKnownAssociations.TypeFinder),
                result.GetAssociation<IAssemblies>(WellKnownAssociations.Assemblies),
                result.GetAssociation<IBindingCollection>(WellKnownAssociations.Bindings),
                result.BootStageResults);
        }
    }
}
