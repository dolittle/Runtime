// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// An <see cref="Module"/> that binds the untyped <see cref="ILogger"/> to the correctly typed <see cref="ILogger{T}"/> when constructing components.
    /// </summary>
    public class LoggerModule : Module
    {
        /// <summary>
        /// Attaches to the <see cref="IComponentRegistration.Preparing"/> event to provide the correctly typed <see cref="ILogger{T}"/> of the type of component being constructed, when an untyped <see cref="ILogger"/> is used as a dependency.
        /// </summary>
        /// <param name="registration">The <see cref="IComponentRegistration"/> to attach to.</param>
        internal static void ResolveUntypedLoggersFor(IComponentRegistration registration)
        {
            registration.Preparing += OnComponentPreparing;
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
            => ResolveUntypedLoggersFor(registration);

        static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Append(
                new ResolvedParameter(
                    (p, _) => p.ParameterType == typeof(ILogger),
                    (p, c) => c.Resolve(typeof(ILogger<>).MakeGenericType(p.Member.DeclaringType))));
        }
    }
}