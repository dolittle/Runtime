// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Serialization.Json;

/// <summary>
/// Indicates that a <see cref="ISerializer" /> is required and can be added as a dependency though the Add method.
/// </summary>
public interface IRequireSerializer
{
    /// <summary>
    /// Adds an instance of an <see cref="ISerializer" />.
    /// </summary>
    /// <param name="serializer"><see cref="ISerializer"/> instance.</param>
    void Add(ISerializer serializer);
}