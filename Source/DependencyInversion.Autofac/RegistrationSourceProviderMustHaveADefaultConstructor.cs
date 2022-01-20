// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Autofac;

/// <summary>
/// Exception that gets thrown when a <see cref="ICanProvideRegistrationSources"/> does not have a default constructor.
/// </summary>
public class RegistrationSourceProviderMustHaveADefaultConstructor : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationSourceProviderMustHaveADefaultConstructor"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> of <see cref="ICanProvideRegistrationSources"/> that is missing default constructor.</param>
    public RegistrationSourceProviderMustHaveADefaultConstructor(Type type)
        : base($"Registration source '{type.AssemblyQualifiedName}' is missing a default constructor")
    {
    }
}