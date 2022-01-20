// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the time spent by a Client attempting to perform a handshake with a Runtime.
/// </summary>
/// <param name="Duration">The time since the first handshake request.</param>
public record HandshakeTimeSpent(TimeSpan Duration) : ConceptAs<TimeSpan>(Duration)
{
    /// <summary>
    /// Implicitly convert from a <see cref="TimeSpan"/> to an <see cref="HandshakeTimeSpent"/>.
    /// </summary>
    /// <param name="duration">HandshakeTimeSpent as a <see cref="TimeSpan"/>.</param>
    /// <returns>The <see cref="HandshakeTimeSpent"/> concept value.</returns>
    public static implicit operator HandshakeTimeSpent(TimeSpan duration) => new(duration);
}
