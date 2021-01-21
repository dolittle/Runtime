// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Autofac.Core;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Defines a provider of <see cref="IRegistrationSource"/> implementations.
    /// </summary>
    public interface ICanProvideRegistrationSources
    {
        /// <summary>
        /// Method that gets called for providing <see cref="IRegistrationSource">registration sources</see>.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <seee cref="IRegistrationSource"/>.</returns>
        IEnumerable<IRegistrationSource> Provide();
    }
}