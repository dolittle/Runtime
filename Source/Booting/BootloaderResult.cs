// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents the result of the <see cref="Bootloader"/> start.
    /// </summary>
    public class BootloaderResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootloaderResult"/> class.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> configured.</param>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> configured.</param>
        /// <param name="assemblies"><see cref="IAssemblies"/> configured.</param>
        /// <param name="bindings"><see cref="IBindingCollection"/> configured.</param>
        /// <param name="bootStageResults"><see cref="BootStageResults">Results from each boot stage</see>.</param>
        public BootloaderResult(
            IContainer container,
            ITypeFinder typeFinder,
            IAssemblies assemblies,
            IBindingCollection bindings,
            IEnumerable<BootStageResult> bootStageResults)
        {
            Container = container;
            TypeFinder = typeFinder;
            Assemblies = assemblies;
            Bindings = bindings;
            BootStageResults = bootStageResults;
        }

        /// <summary>
        /// Gets the <see cref="IContainer"/> configured.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// Gets the <see cref="ITypeFinder"/> configured.
        /// </summary>
        public ITypeFinder TypeFinder { get; }

        /// <summary>
        /// Gets the <see cref="IAssemblies"/> configured.
        /// </summary>
        public IAssemblies Assemblies { get; }

        /// <summary>
        /// Gets the <see cref="IBindingCollection">bindings</see> configured.
        /// </summary>
        public IBindingCollection Bindings { get; }

        /// <summary>
        /// Gets the <see cref="BootStageResults">results from each boot stage</see>.
        /// </summary>
        public IEnumerable<BootStageResult> BootStageResults { get; }
    }
}