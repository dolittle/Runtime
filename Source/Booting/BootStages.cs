// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Booting.Stages;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Reflection;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents an implementation of <see cref="IBootStages"/>.
    /// </summary>
    public class BootStages : IBootStages
    {
        static IContainer _container = null;
        readonly IEnumerable<ICanPerformPartOfBootStage> _initialFixedStages;

        readonly Queue<ICanPerformPartOfBootStage> _stages;
        ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootStages"/> class.
        /// </summary>
        public BootStages()
        {
            _initialFixedStages = new ICanPerformPartOfBootStage[]
            {
                new Basics(),
                new Logging(),
                new Discovery(),
                new PostDiscovery(DiscoverBootStages)
            };
            _stages = new Queue<ICanPerformPartOfBootStage>(_initialFixedStages);
        }

        /// <summary>
        /// Method that gets called when <see cref="IContainer"/> is ready.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> instance.</param>
        public static void ContainerReady(IContainer container) => _container = container;

        /// <inheritdoc/>
        public BootStagesResult Perform(Boot boot)
        {
            var newBindingsNotificationHub = new NewBindingsNotificationHub();
            var results = new List<BootStageResult>();
            var aggregatedAssociations = new Dictionary<string, object>()
            {
                { WellKnownAssociations.NewBindingsNotificationHub, newBindingsNotificationHub }
            };
            IBindingCollection bindingCollection = new BindingCollection(new[]
            {
                new BindingBuilder(Binding.For(typeof(GetContainer))).To((GetContainer)(() => _container)).Build()
            });

            aggregatedAssociations[WellKnownAssociations.Bindings] = bindingCollection;

            while (_stages.Count > 0)
            {
                var stage = _stages.Dequeue();
                
                SetupLoggerIfAssociated(aggregatedAssociations);

                var interfaces = stage.GetType().GetInterfaces();
                
                var isBefore = interfaces.Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ICanRunBeforeBootStage<>));
                var isAfter = interfaces.Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ICanRunAfterBootStage<>));

                var suffix = string.Empty;
                if (isBefore) suffix = " (before)";
                if (isAfter) suffix = " (after)";

                _logger?.LogDebug($"<********* BOOTSTAGE : {stage.BootStage}{suffix} *********>");

                var performer = interfaces.SingleOrDefault(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ICanPerformPartOfBootStage<>));
                var settingsType = performer.GetGenericArguments()[0];
                var settings = boot.GetSettingsByType(settingsType);
                var method = performer.GetMethod("Perform", BindingFlags.Public | BindingFlags.Instance);

                aggregatedAssociations[WellKnownAssociations.Bindings] = bindingCollection;
                var builder = new BootStageBuilder(container: _container, initialAssociations: aggregatedAssociations);
                method.Invoke(stage, new object[] { settings, builder });
                var result = builder.Build();
                results.Add(result);

                result.Associations.ForEach(_ => aggregatedAssociations[_.Key] = _.Value);

                newBindingsNotificationHub.Notify(result.Bindings);
                bindingCollection = aggregatedAssociations[WellKnownAssociations.Bindings] as IBindingCollection;
                bindingCollection = new BindingCollection(bindingCollection, result.Bindings);
                _container = result.Container;
            }

            return new BootStagesResult(_container, aggregatedAssociations, results);
        }

        void SetupLoggerIfAssociated(Dictionary<string, object> aggregatedAssociations)
        {
            if (aggregatedAssociations.ContainsKey(WellKnownAssociations.LoggerFactory))
            {
                var loggerFactory = aggregatedAssociations[WellKnownAssociations.LoggerFactory] as ILoggerFactory;
                _logger = loggerFactory.CreateLogger<BootStages>();
            }
        }

        void DiscoverBootStages(ITypeFinder typeFinder)
        {
            var bootStagePerformerTypes = typeFinder.FindMultiple<ICanPerformPartOfBootStage>();
            var bootStagePerformers = bootStagePerformerTypes
                .Where(_ => !_initialFixedStages.Any(existing => existing.GetType() == _))
                .Select(_ =>
                {
                    ThrowIfMissingDefaultConstructorForBootStagePerformer(_);
                    if (_container != null) return _container.Get(_) as ICanPerformPartOfBootStage;
                    return Activator.CreateInstance(_) as ICanPerformPartOfBootStage;
                })
                .OrderBy(_ => _.BootStage);

            bootStagePerformers.GroupBy(performer => performer.BootStage).ForEach(performers =>
            {
                var beforePerformers = performers.Where(_ => HasInterface(_, typeof(ICanRunBeforeBootStage<>)));
                beforePerformers.ForEach(_stages.Enqueue);

                var performer = performers.Single(_ => HasInterface(_, typeof(ICanPerformBootStage<>)));
                _stages.Enqueue(performer);

                var afterPerformers = performers.Where(_ => HasInterface(_, typeof(ICanRunAfterBootStage<>)));
                afterPerformers.ForEach(_stages.Enqueue);
            });

            ThrowIfMissingBootStage(bootStagePerformers);
        }

        static void ThrowIfMissingDefaultConstructorForBootStagePerformer(Type type)
        {
            if (!type.HasDefaultConstructor()) throw new MissingDefaultConstructorForBootStagePerformer(type);
        }

        void ThrowIfMissingBootStage(IEnumerable<ICanPerformPartOfBootStage> performers)
        {
            var bootStageValues = Enum
                .GetValues(typeof(BootStage))
                .Cast<BootStage>()
                .Where(_ => !_initialFixedStages.Any(existing => existing.BootStage == _));

            bootStageValues.ForEach(bootStage =>
            {
                var hasPerformer = performers.Any(performer =>
                    {
                        var interfaces = performer.GetType().GetInterfaces();

                        var isBefore = interfaces.Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ICanRunBeforeBootStage<>));
                        var isAfter = interfaces.Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == typeof(ICanRunAfterBootStage<>));
                        if (!isBefore && !isAfter) return performer.BootStage == bootStage;
                        return false;
                    });
                if (!hasPerformer) throw new MissingBootStage(bootStage);
            });
        }

        static bool HasInterface(ICanPerformPartOfBootStage performer, Type interfaceType)
            => performer
                .GetType()
                .GetInterfaces()
                .Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == interfaceType);
    }
}