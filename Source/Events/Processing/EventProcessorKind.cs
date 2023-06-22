// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing;

/// <summary>
/// Represents the kind of an <see cref="IEventProcessor"/>.
/// </summary>
public record EventProcessorKind(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="EventProcessorKind"/>.
    /// </summary>
    /// <param name="identifier"><see cref="string"/> representation.</param>
    public static implicit operator EventProcessorKind(string identifier) => new(identifier);
    
    public static readonly EventProcessorKind Actor = "Actor";
}
