// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Domain.Platform;

/// <summary>
/// Represents the concept of an application.
/// </summary>
public record ApplicationId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Represents the identifier for a not set application.
    /// </summary>
    public static readonly ApplicationId NotSet = Guid.Parse("4fe9492c-1d19-4e6b-be72-03208789906e");

    /// <summary>
    /// Implicitly converts from a <see cref="Guid"/> to a <see cref="ApplicationId"/>.
    /// </summary>
    /// <param name="application"><see cref="Guid"/> representing the application.</param>
    public static implicit operator ApplicationId(Guid application) => new(application);
    
    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to a <see cref="ApplicationId"/>.
    /// </summary>
    /// <param name="application"><see cref="string"/> representing the application.</param>
    public static implicit operator ApplicationId(string application) => new(Guid.Parse(application));
}
