// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Logging;

/// <summary>
/// Represents an implementation of <see cref="Module"/> that adds resolving middleware to all registered types that have <see cref="Microsoft.Extensions.Logging"/> dependencies.
/// </summary>
public class LoggerModule : Module
{
    /// <inheritdoc />
    protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
    {
        if (registration.Activator is not ReflectionActivator reflectionActivator)
        {
            return;
        }

        if (!AnyConstructorHasLoggerDependency(reflectionActivator))
        {
            return;
        }

        registration.PipelineBuilding += (_, builder) => builder.Use(new LoggerResolvingMiddleware(reflectionActivator.LimitType));
    }

    static bool AnyConstructorHasLoggerDependency(ReflectionActivator activator)
        => activator
            .ConstructorFinder.FindConstructors(activator.LimitType)
            .SelectMany(constructor => constructor.GetParameters().Select(_ => _.ParameterType))
            .Any(parameterType => parameterType == typeof(ILogger));
}
