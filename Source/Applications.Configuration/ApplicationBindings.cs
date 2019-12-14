// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Execution;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Binds up the bindings related to the running application. The<see cref="Application"/>, the <see cref="BoundedContext"/> and the <see cref="Environment"/>.
    /// </summary>
    public class ApplicationBindings : ICanProvideBindings
    {
        readonly BoundedContextConfiguration _boundedContextConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBindings"/> class.
        /// </summary>
        /// <param name="boundedContextConfiguration"><see cref="BoundedContextConfiguration">Configuration</see> for the bounded context.</param>
        public ApplicationBindings(BoundedContextConfiguration boundedContextConfiguration)
        {
            _boundedContextConfiguration = boundedContextConfiguration;
        }

        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<Application>().To(() => _boundedContextConfiguration.Application);
            builder.Bind<BoundedContext>().To(() => _boundedContextConfiguration.BoundedContext);
        }
    }
}