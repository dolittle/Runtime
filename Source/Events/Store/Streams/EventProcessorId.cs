// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing;

/// <summary>
/// Represents a unique identifier for a <see cref="IEventProcessor"/>.
/// </summary>
public record EventProcessorId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Implicitly convert from <see cref="Guid"/> to <see cref="EventProcessorId"/>.
    /// </summary>
    /// <param name="identifier"><see cref="Guid"/> representation.</param>
    public static implicit operator EventProcessorId(Guid identifier) => new(identifier);

    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="EventProcessorId"/>.
    /// </summary>
    /// <param name="identifier"><see cref="string"/> representation.</param>
    public static implicit operator EventProcessorId(string identifier) => Guid.Parse(identifier);
}