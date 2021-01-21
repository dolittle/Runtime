// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Build.CLI
{
    /// <summary>
    /// Represents bindings for the build system.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            var buildTarget = Program.BuildTarget ?? new BuildTarget(null, null, null, null);
            builder.Bind<BuildTarget>().To(buildTarget);
        }
    }
}