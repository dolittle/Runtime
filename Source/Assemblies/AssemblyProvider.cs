// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extension.Logging;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents an implementation of <see cref="IAssemblyProvider"/>.
    /// </summary>
    [Singleton]
    public class AssemblyProvider : IAssemblyProvider
    {
        static readonly object _lockObject = new object();
        readonly AssemblyComparer comparer = new AssemblyComparer();
        readonly IEnumerable<ICanProvideAssemblies> _assemblyProviders;
        readonly IAssemblyFilters _assemblyFilters;
        readonly IAssemblyUtility _assemblyUtility;
        readonly Dictionary<string, Library> _libraries = new Dictionary<string, Library>();
        readonly List<Assembly> _assemblies = new List<Assembly>();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyProvider"/> class.
        /// </summary>
        /// <param name="assemblyProviders"><see cref="IEnumerable{ICanProvideAssemblies}">Providers</see> to provide assemblies.</param>
        /// <param name="assemblyFilters"><see cref="IAssemblyFilters"/> to use for filtering assemblies through.</param>
        /// <param name="assemblyUtility">An <see cref="IAssemblyUtility"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public AssemblyProvider(
            IEnumerable<ICanProvideAssemblies> assemblyProviders,
            IAssemblyFilters assemblyFilters,
            IAssemblyUtility assemblyUtility,
            ILogger logger)
        {
            _assemblyProviders = assemblyProviders;
            _assemblyFilters = assemblyFilters;
            _assemblyUtility = assemblyUtility;
            _logger = logger;

            Populate();
        }

        /// <inheritdoc/>
        public IEnumerable<Assembly> GetAll()
        {
            return _assemblies;
        }

        void Populate()
        {
            foreach (var provider in _assemblyProviders)
            {
                provider.Libraries.ForEach(library =>
                {
                    if (!_libraries.ContainsKey(library.Name)) _libraries.Add(library.Name, library);
                });

                var assembliesToInclude = provider.Libraries.Where(
                    library =>
                        _assemblyFilters.ShouldInclude(library) &&
                        _assemblyUtility.IsAssembly(library));

                var filtered = assembliesToInclude.ToArray();

                assembliesToInclude.Select(provider.GetFrom).ForEach(AddAssembly);
            }
        }

        void ReapplyFilter()
        {
            var assembliesToRemove = _assemblies.Where(a =>
            {
                var name = a.GetName().Name;
                if (!_libraries.ContainsKey(name)) return true;
                return !_assemblyFilters.ShouldInclude(_libraries[name]);
            }).ToArray();
            assembliesToRemove.ForEach((a) => _assemblies.Remove(a));
        }

        void AddAssembly(Assembly assembly)
        {
            lock (_lockObject)
            {
                if (!_assemblies.Contains(assembly, comparer) &&
                    !_assemblyUtility.IsDynamic(assembly))
                {
                    _assemblies.Add(assembly);
                    ReapplyFilter();
                }
            }
        }
    }
}
