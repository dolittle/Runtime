// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Scopes
{
    /// <summary>
    /// Represents a <see cref="IScope"/> for singleton per tenant - one instance per process per tenant
    /// Adhering to the highlander principle; there can be only one - per planet. I digress.
    /// </summary>
    public class SingletonPerTenant : IScope
    {
    }
}