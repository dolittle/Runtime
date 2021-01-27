// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Applications.Configuration
{
    /// <summary>
    /// Represents a configuration that describes which <see cref="ResourceTypeImplementation"/> that's configured for a <see cref="ResourceType"/>. Used in <see cref="BoundedContextConfiguration"/> for serialization .
    /// </summary>
    public record ResourceTypeImplementationConfiguration(string Production, string Development);
}