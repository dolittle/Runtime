// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents an implementation of <see cref="IBootStageBuilder"/>.
    /// </summary>
    public class BootStageBuilder : IBootStageBuilder
    {
        readonly Dictionary<string, object> _initialAssociations;
        readonly Dictionary<string, object> _associations = new();
        IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootStageBuilder"/> class.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> to use for the stage building - optional, can be null.</param>
        /// <param name="initialAssociations"><see cref="IDictionary{TKey, TValue}"/> with initial associations.</param>
        public BootStageBuilder(IContainer container = null, IDictionary<string, object> initialAssociations = null)
        {
            _container = container;
            if (initialAssociations != null) _initialAssociations = new Dictionary<string, object>(initialAssociations);
            else _initialAssociations = new Dictionary<string, object>();
            Bindings = new BindingProviderBuilder();
        }

        /// <inheritdoc/>
        public IBindingProviderBuilder Bindings { get; }

        /// <inheritdoc/>
        public IContainer Container
        {
            get
            {
                ThrowIfContainerIsNotSet();
                return _container;
            }
        }

        /// <inheritdoc/>
        public void Associate(string key, object value) => _associations[key] = value;

        /// <inheritdoc/>
        public object GetAssociation(string key)
        {
            if (_associations.ContainsKey(key)) return _associations[key];
            if (_initialAssociations.ContainsKey(key)) return _initialAssociations[key];

            throw new MissingAssociation(key);
        }

        /// <inheritdoc/>
        public T GetAssociation<T>(string key) where T : class => GetAssociation(key) as T;

        /// <inheritdoc/>
        public BootStageResult Build() => new(_container, Bindings.Build(), _associations);

        /// <inheritdoc/>
        public void UseContainer(IContainer container) => _container = container;

        void ThrowIfContainerIsNotSet()
        {
            if (_container == null) throw new ContainerNotSetYet();
        }
    }
}