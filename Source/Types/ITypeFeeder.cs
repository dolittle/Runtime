// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Assemblies;

namespace Dolittle.Runtime.Types
{
    /// <summary>
    /// Defines a system that is capable of feeding types from <see cref="IAssemblies"/> to
    /// <see cref="IContractToImplementorsMap"/>.
    /// </summary>
    public interface ITypeFeeder
    {
        /// <summary>
        /// Feed types from <see cref="IAssemblies"/> to <see cref="IContractToImplementorsMap"/>.
        /// </summary>
        /// <param name="assemblies"><see cref="IAssemblies"/> to feed types from.</param>
        /// <param name="map"><see cref="IContractToImplementorsMap"/> to feed to.</param>
        void Feed(IAssemblies assemblies, IContractToImplementorsMap map);
    }
}
