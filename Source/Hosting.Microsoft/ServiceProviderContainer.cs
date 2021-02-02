// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Hosting.Microsoft
{
    /// <summary>
    /// Represents an implementation of <see cref="IContainer"/> using an <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceProviderContainer : IContainer
    {
        readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderContainer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <cee cref="IServiceProvider"/> to use for getting instances of types.</param>
        public ServiceProviderContainer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        /// <inheritdoc/>
        public T Get<T>() => (T)_serviceProvider.GetService(typeof(T));

        /// <inheritdoc/>
        public object Get(Type type) => _serviceProvider.GetService(type);
    }
}
