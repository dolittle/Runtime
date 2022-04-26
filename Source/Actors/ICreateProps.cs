// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Proto;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Defines a system that can create <see cref="Props"/> for actors using the Dolittle Runtime dependency inversion system.
/// </summary>
public interface ICreateProps
{
    /// <summary>
    /// Creates the <see cref="Props"/> for an actor using the tenant specific IoC Container.
    /// </summary>
    /// <param name="parameters">The optional list of parameters to use when constructing the actor object.</param>
    /// <typeparam name="TActor">The <see cref="Type"/> of the actor.</typeparam>
    /// <returns>The <see cref="Props"/>.</returns>
    Props PropsFor<TActor>(params object[] parameters)
        where TActor : IActor;
}
