// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// A delegate representing something that can create instances, typically by leveraging the <see cref="IContainer"/>.
    /// </summary>
    /// <typeparam name="T">Type to be factory for.</typeparam>
    /// <returns>Instance of the type asked for.</returns>
    public delegate T FactoryFor<T>();
}