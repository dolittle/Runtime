// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Booting;
using Dolittle.Runtime.DependencyInversion.Conventions;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Booting;

/// <summary>
/// The entrypoint for DependencyInversion.
/// </summary>
public static class Boot
{
    static IContainer _container;

    /// <summary>
    /// Initialize the entire DependencyInversion pipeline.
    /// </summary>
    /// <param name="assemblies"><see cref="IAssemblies"/> for the application.</param>
    /// <param name="typeFinder"><see cref="ITypeFinder"/> for doing discovery.</param>
    /// <param name="bindings">Additional bindings.</param>
    /// <param name="bootContainer">A <see cref="BootContainer"/> used during booting.</param>
    /// <returns>Configured <see cref="IContainer"/> and <see cref="IBindingCollection"/>.</returns>
    public static BootResult Start(
        IAssemblies assemblies,
        ITypeFinder typeFinder,
        ILoggerFactory loggerFactory,
        IEnumerable<Binding> bindings = null,
        BootContainer bootContainer = null)
    {
        var logger = loggerFactory.CreateLogger(typeof(Boot));
        Log.StartDependencyInversion(logger);
        var initialBindings = GetBootBindings(assemblies, typeFinder, loggerFactory);
        bootContainer ??= new BootContainer(initialBindings, new NewBindingsNotificationHub());
        _container = bootContainer;

        var otherBindings = new List<Binding>();

        if (bindings != null)
        {
            otherBindings.AddRange(bindings);
        }
        otherBindings.Add(Bind(typeof(IContainer), () => _container, false));

        Log.DiscoverAndBuildBindings(logger);
        var bindingCollection = DiscoverAndBuildBuildBindings(
            bootContainer,
            typeFinder,
            logger,
            initialBindings,
            otherBindings);

        Log.DiscoverContainer(logger);
        _container = DiscoverAndConfigureContainer(bootContainer, assemblies, typeFinder, bindingCollection);
        BootContainer.ContainerReady(_container);

        Log.ReturnBootResult(logger);
        return new BootResult(_container, bindingCollection);
    }

    /// <summary>
    /// Initialize the entire DependencyInversion pipeline with a specified <see cref="Type"/> of container.
    /// </summary>
    /// <param name="assemblies"><see cref="IAssemblies"/> for the application.</param>
    /// <param name="typeFinder"><see cref="ITypeFinder"/> for doing discovery.</param>
    /// <param name="containerType"><see cref="Type"/>Container type.</param>
    /// <param name="bindings">Additional bindings.</param>
    /// <param name="bootContainer">A <see cref="BootContainer"/> used during booting.</param>
    /// <returns>Configured <see cref="IContainer"/> and <see cref="IBindingCollection"/>.</returns>
    public static IBindingCollection Start(
        IAssemblies assemblies,
        ITypeFinder typeFinder,
        ILoggerFactory loggerFactory,
        Type containerType,
        IEnumerable<Binding> bindings = null,
        BootContainer bootContainer = null)
    {
        var logger = loggerFactory.CreateLogger(typeof(Boot));
        Log.StartDependencyInversion(logger);
        var initialBindings = GetBootBindings(assemblies, typeFinder, loggerFactory);

        bootContainer ??= new BootContainer(initialBindings, new NewBindingsNotificationHub());
        _container = bootContainer;

        var otherBindings = new List<Binding>();

        if (bindings != null)
        {
            otherBindings.AddRange(bindings);
        }
        otherBindings.Add(Bind(typeof(IContainer), containerType, true));

        Log.DiscoverAndBuildBindings(logger);
        return DiscoverAndBuildBuildBindings(
            bootContainer,
            typeFinder,
            logger,
            initialBindings,
            otherBindings);
    }

    /// <summary>
    /// Method that gets called when <see cref="IContainer"/> is ready.
    /// </summary>
    /// <param name="container"><see cref="IContainer"/> instance.</param>
    public static void ContainerReady(IContainer container)
    {
        _container = container;
        BootContainer.ContainerReady(container);
    }

    static IBindingCollection GetBootBindings(
        IAssemblies assemblies,
        ITypeFinder typeFinder,
        ILoggerFactory loggerFactory)
    {
        return new BindingCollection(new[]
        {
            Bind(typeof(IAssemblies), assemblies),
            Bind(typeof(ITypeFinder), typeFinder),
            Bind(typeof(ILoggerFactory), loggerFactory),
            Bind(typeof(GetContainer), new GetContainer(() => _container))
        });
    }

    static IBindingCollection DiscoverAndBuildBuildBindings(
        IContainer bootContainer,
        ITypeFinder typeFinder,
        ILogger logger,
        IBindingCollection initialBindings,
        IEnumerable<Binding> bindings)
    {
        Log.DiscoverBindings(logger);
        var discoveredBindings = DiscoverBindings(bootContainer, typeFinder, logger);

        Log.CreateNewBindingCollection(logger);
        var bindingCollection = new BindingCollection(initialBindings, discoveredBindings, bindings);

        foreach (var binding in bindingCollection)
        {
            Log.DiscoveredBinding(logger, binding.Service.AssemblyQualifiedName, binding.Strategy.GetType().Name);
        }

        var asmBindings = bindingCollection.Where(_ => _.Service == typeof(IAssemblies)).ToArray();

        return bindingCollection;
    }

    static IBindingCollection DiscoverBindings(
        IContainer bootContainer,
        ITypeFinder typeFinder,
        ILogger logger)
    {
        Log.DiscoverBindings(logger);
        var bindingConventionManager = new BindingConventionManager(bootContainer, typeFinder, logger);

        Log.DiscoverAndSetupBindings(logger);
        var bindingsFromConventions = bindingConventionManager.DiscoverAndSetupBindings();

        Log.DiscoverBindingProviders(logger);
        var bindingsFromProviders = DiscoverBindingProvidersAndGetBindings(bootContainer, typeFinder);

        Log.ComposeBindings(logger);
        return new BindingCollection(bindingsFromProviders, bindingsFromConventions);
    }

    static Binding Bind(Type type, Type target, bool singleton = false)
    {
        var containerBindingBuilder = new BindingBuilder(Binding.For(type));
        var scope = containerBindingBuilder.To(target);
        if (singleton)
        {
            scope.Singleton();
        }
        return containerBindingBuilder.Build();
    }

    static Binding Bind(Type type, object target)
    {
        var containerBindingBuilder = new BindingBuilder(Binding.For(type));
        var scope = containerBindingBuilder.To(target);
        scope.Singleton();
        return containerBindingBuilder.Build();
    }

    static Binding Bind(Type type, Func<object> target, bool singleton = false)
    {
        var containerBindingBuilder = new BindingBuilder(Binding.For(type));
        var scope = containerBindingBuilder.To(target);
        if (singleton)
        {
            scope.Singleton();
        }
        return containerBindingBuilder.Build();
    }

    static IBindingCollection DiscoverBindingProvidersAndGetBindings(
        IContainer bootContainer,
        ITypeFinder typeFinder)
    {
        var bindingProviders = typeFinder.FindMultiple<ICanProvideBindings>();
        var bindingCollections = new ConcurrentBag<IBindingCollection>();

        Parallel.ForEach(
            bindingProviders,
            bindingProviderType => ProvideBindingsFromProvider(bindingProviderType, bootContainer, bindingCollections));

        var bindingCollection = new BindingCollection(bindingCollections.ToArray());
        return bindingCollection;
    }

    static IContainer DiscoverAndConfigureContainer(
        IContainer bootContainer,
        IAssemblies assemblies,
        ITypeFinder typeFinder,
        IBindingCollection bindingCollection)
    {
        var containerProviderType = typeFinder.FindSingle<ICanProvideContainer>();
        ThrowIfMissingContainerProvider(containerProviderType);

        var containerProvider = bootContainer.Get(containerProviderType) as ICanProvideContainer;

        var container = containerProvider.Provide(assemblies, bindingCollection);
        return container;
    }

    static void ProvideBindingsFromProvider(Type bindingProviderType, IContainer bootContainer, ConcurrentBag<IBindingCollection> bindingCollections)
    {
        var bindingProvider = bootContainer.Get(bindingProviderType) as ICanProvideBindings;
        var bindingProviderBuilder = new BindingProviderBuilder();
        bindingProvider.Provide(bindingProviderBuilder);
        bindingCollections.Add(bindingProviderBuilder.Build());
    }

    static void ThrowIfMissingContainerProvider(Type containerProvider)
    {
        if (containerProvider == null)
        {
            throw new MissingContainerProvider();
        }
    }
}
