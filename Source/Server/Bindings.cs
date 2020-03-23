// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Runtime.Logging.Management;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanProvideBindings"/>.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            ManagementLogAppender.LogManager = new LogManager();
            builder.Bind<ILogManager>().To(ManagementLogAppender.LogManager);
        }
    }
}