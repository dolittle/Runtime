// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Represents an implementation of <see cref="IPolicies"/>.
    /// </summary>
    /// <remarks>
    /// For each type of policy there can only be one definition. This means there can be only
    /// one defined for each specific type, one for given name or one specifying the default.
    /// </remarks>
    [Singleton]
    public class Policies : IPolicies
    {
        readonly IInstancesOf<IDefineDefaultPolicy> _defaultPolicyDefiners;
        readonly IInstancesOf<IDefineDefaultAsyncPolicy> _defaultAsyncPolicyDefiners;
        readonly IInstancesOf<IDefineNamedPolicy> _namedPolicyDefiners;
        readonly IInstancesOf<IDefineNamedAsyncPolicy> _namedAsyncPolicyDefiners;
        readonly IInstancesOf<IDefinePolicyForType> _typedPolicyDefiners;
        readonly IInstancesOf<IDefineAsyncPolicyForType> _typedAsyncPolicyDefiners;
        readonly IDictionary<string, INamedPolicy> _namedPolicies = new Dictionary<string, INamedPolicy>();
        readonly IDictionary<string, INamedAsyncPolicy> _namedAsyncPolicies = new Dictionary<string, INamedAsyncPolicy>();
        readonly IDictionary<Type, IPolicy> _typedPolicies = new Dictionary<Type, IPolicy>();
        readonly IDictionary<Type, IAsyncPolicy> _typedAsyncPolicies = new Dictionary<Type, IAsyncPolicy>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Policies"/> class.
        /// </summary>
        /// <param name="defaultPolicyDefiners">Instances of <see cref="IDefineDefaultPolicy">default policy definers</see>.</param>
        /// <param name="defaultAsyncPolicyDefiners">Instances of <see cref="IDefineDefaultAsyncPolicy">default async policy definers</see>.</param>
        /// <param name="namedPolicyDefiners">Instances of <see cref="IDefineNamedPolicy">named policy definers</see>.</param>
        /// <param name="namedAsyncPolicyDefiners">Instances of <see cref="IDefineNamedAsyncPolicy">named async policy definers</see>.</param>
        /// <param name="typedPolicyDefiners">Instances of <see cref="IDefinePolicyForType">typed policy definers</see>.</param>
        /// <param name="typedAsyncPolicyDefiners">Instances of <see cref="IDefineAsyncPolicyForType">typed async policy definers</see>.</param>
        public Policies(
            IInstancesOf<IDefineDefaultPolicy> defaultPolicyDefiners,
            IInstancesOf<IDefineDefaultAsyncPolicy> defaultAsyncPolicyDefiners,
            IInstancesOf<IDefineNamedPolicy> namedPolicyDefiners,
            IInstancesOf<IDefineNamedAsyncPolicy> namedAsyncPolicyDefiners,
            IInstancesOf<IDefinePolicyForType> typedPolicyDefiners,
            IInstancesOf<IDefineAsyncPolicyForType> typedAsyncPolicyDefiners)
        {
            _defaultPolicyDefiners = defaultPolicyDefiners;
            _defaultAsyncPolicyDefiners = defaultAsyncPolicyDefiners;
            _namedPolicyDefiners = namedPolicyDefiners;
            _namedAsyncPolicyDefiners = namedAsyncPolicyDefiners;
            _typedPolicyDefiners = typedPolicyDefiners;
            _typedAsyncPolicyDefiners = typedAsyncPolicyDefiners;

            ThrowIfMultipleDefaultPoilicyDefinersAreFound();
            Default = DefineDefaultPolicy();
            DefaultAsync = DefineDefaultAsyncPolicy();
            PopulateNamedPolicies();
            PopulateNamedAsyncPolicies();
            PopulateTypedPolicies();
            PopulateTypedAsyncPolicies();
        }

        /// <inheritdoc/>
        public IPolicy Default { get; }

        /// <inheritdoc/>
        public IAsyncPolicy DefaultAsync { get; }

        /// <inheritdoc/>
        public INamedPolicy GetNamed(string name)
        {
            if (_namedPolicies.ContainsKey(name)) return _namedPolicies[name];
            var policy = new NamedPolicy(name, Default);
            _namedPolicies[name] = policy;
            return policy;
        }

        /// <inheritdoc/>
        public INamedAsyncPolicy GetAsyncNamed(string name)
        {
            if (_namedAsyncPolicies.ContainsKey(name)) return _namedAsyncPolicies[name];
            var policy = new NamedAsyncPolicy(name, DefaultAsync);
            _namedAsyncPolicies[name] = policy;
            return policy;
        }

        /// <inheritdoc/>
        public IPolicyFor<T> GetFor<T>()
        {
            var type = typeof(T);
            if (_typedPolicies.ContainsKey(type)) return _typedPolicies[type] as IPolicyFor<T>;
            var policyFor = typeof(PolicyFor<>).MakeGenericType(type);

            var constructor = policyFor.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IPolicy) }, new ParameterModifier[] { new ParameterModifier(1) });
            var policy = constructor.Invoke(new[] { Default }) as IPolicyFor<T>;
            _typedPolicies[type] = policy;
            return policy;
        }

        /// <inheritdoc/>
        public IAsyncPolicyFor<T> GetAsyncFor<T>()
        {
            var type = typeof(T);
            if (_typedAsyncPolicies.ContainsKey(type)) return _typedAsyncPolicies[type] as IAsyncPolicyFor<T>;
            var policyFor = typeof(AsyncPolicyFor<>).MakeGenericType(type);

            var constructor = policyFor.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IAsyncPolicy) }, new ParameterModifier[] { new ParameterModifier(1) });
            var policy = constructor.Invoke(new[] { DefaultAsync }) as IAsyncPolicyFor<T>;
            _typedAsyncPolicies[type] = policy;
            return policy;
        }

        IPolicy DefineDefaultPolicy()
        {
            var underlyingPolicy = _defaultPolicyDefiners.FirstOrDefault()?.Define();
            var policy = underlyingPolicy != null ? (IPolicy)new Policy(underlyingPolicy) : new PassThroughPolicy();

            return policy;
        }

        IAsyncPolicy DefineDefaultAsyncPolicy()
        {
            var underlyingPolicy = _defaultAsyncPolicyDefiners.FirstOrDefault()?.Define();
            var policy = underlyingPolicy != null ? (IAsyncPolicy)new AsyncPolicy(underlyingPolicy) : new PassThroughAsyncPolicy();

            return policy;
        }

        void PopulateNamedPolicies()
        {
            _namedPolicyDefiners.ForEach(_ =>
            {
                ThrowIfMultiplePolicyForNameFound(_.Name);
                _namedPolicies[_.Name] = new NamedPolicy(_.Name, _.Define());
            });
        }

        void PopulateNamedAsyncPolicies()
        {
            _namedAsyncPolicyDefiners.ForEach(_ =>
            {
                ThrowIfMultiplePolicyForNameFound(_.Name);
                _namedAsyncPolicies[_.Name] = new NamedAsyncPolicy(_.Name, _.Define());
            });
        }

        void PopulateTypedPolicies()
        {
            _typedPolicyDefiners.ForEach(_ =>
            {
                ThrowIfMultiplePolicyForTypeFound(_.Type);
                var policyFor = typeof(PolicyFor<>).MakeGenericType(_.Type);
                var constructor = policyFor.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Polly.ISyncPolicy) }, new ParameterModifier[] { new ParameterModifier(1) });

                _typedPolicies[_.Type] = constructor.Invoke(new[] { _.Define() }) as IPolicy;
            });
        }

        void PopulateTypedAsyncPolicies()
        {
            _typedAsyncPolicyDefiners.ForEach(_ =>
            {
                ThrowIfMultiplePolicyForTypeFound(_.Type);
                var policyFor = typeof(AsyncPolicyFor<>).MakeGenericType(_.Type);
                var constructor = policyFor.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Polly.IAsyncPolicy) }, new ParameterModifier[] { new ParameterModifier(1) });

                _typedAsyncPolicies[_.Type] = constructor.Invoke(new[] { _.Define() }) as IAsyncPolicy;
            });
        }

        void ThrowIfMultipleDefaultPoilicyDefinersAreFound()
        {
            if (_defaultPolicyDefiners.Count() > 1 || _defaultAsyncPolicyDefiners.Count() > 1) throw new MultipleDefaultPolicyDefinersFound();
        }

        void ThrowIfMultiplePolicyForNameFound(string name)
        {
            if (_namedPolicies.ContainsKey(name) || _namedAsyncPolicies.ContainsKey(name)) throw new MultiplePolicyDefinersForNameFound(name);
        }

        void ThrowIfMultiplePolicyForTypeFound(Type type)
        {
            if (_typedPolicies.ContainsKey(type) || _typedAsyncPolicies.ContainsKey(type)) throw new MultiplePolicyDefinersForTypeFound(type);
        }
    }
}