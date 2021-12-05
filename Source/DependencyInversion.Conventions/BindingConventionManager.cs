// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Types;
using System.Threading.Tasks;

namespace Dolittle.Runtime.DependencyInversion.Conventions;

/// <summary>
/// Represents a <see cref="IBindingConventionManager"/>.
/// </summary>
[Singleton]
public class BindingConventionManager : IBindingConventionManager
{
    readonly ITypeFinder _typeFinder;
    readonly ILogger _logger;
    readonly IContainer _bootContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingConventionManager"/> class.
    /// </summary>
    /// <param name="bootContainer"><see cref="IContainer"/> used during booting.</param>
    /// <param name="typeFinder"><see cref="ITypeFinder"/> to discover binding conventions with.</param>
    /// <param name="logger"><see cref="ILogger"/> used for logging.</param>
    public BindingConventionManager(
        IContainer bootContainer,
        ITypeFinder typeFinder,
        ILogger logger)
    {
        _typeFinder = typeFinder;
        _logger = logger;
        _bootContainer = bootContainer;
    }

    /// <inheritdoc/>
    public IBindingCollection DiscoverAndSetupBindings()
    {
        Log.DiscoverAndSetupBindings(_logger);
        var bindingCollections = new ConcurrentBag<IBindingCollection>();

        var allTypes = _typeFinder.All;

        Log.FindAllBindingConventions(_logger);
        var conventionTypes = _typeFinder.FindMultiple<IBindingConvention>();

        Parallel.ForEach(conventionTypes, conventionType => HandleConvention(conventionType, allTypes, bindingCollections));

        return new BindingCollection(bindingCollections.ToArray());
    }

    void HandleConvention(
        Type conventionType,
        IEnumerable<Type> allTypes,
        ConcurrentBag<IBindingCollection> bindingCollections)
    {
        Log.HandleConvention(_logger, conventionType.AssemblyQualifiedName);

        var convention = _bootContainer.Get(conventionType) as IBindingConvention;
        var servicesToResolve = allTypes.Where(service => convention.CanResolve(service));

        var bindings = new ConcurrentBag<Binding>();

        Parallel.ForEach(
            servicesToResolve,
            service => ResolveAndAddConventionBindingsForService(service, convention, bindings));

        var bindingCollection = new BindingCollection(bindings);
        bindingCollections.Add(bindingCollection);
    }
    static void ResolveAndAddConventionBindingsForService(Type service, IBindingConvention convention, ConcurrentBag<Binding> bindings)
    {
        var bindingBuilder = new BindingBuilder(Binding.For(service));
        convention.Resolve(service, bindingBuilder);
        bindings.Add(bindingBuilder.Build());
    }
}
