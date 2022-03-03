// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Domain.Platform;

/// <summary>
/// Represents the name of <see cref="ApplicationId"/>.
/// </summary>
public record ApplicationName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="ApplicationName"/> representing an undefined name.
    /// </summary>
    public static readonly ApplicationName NotSet = "[Not Set]";

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to a <see cref="ApplicationName"/>.
    /// </summary>
    /// <param name="applicationName">Name of the <see cref="ApplicationName"/>.</param>
    public static implicit operator ApplicationName(string applicationName) => new(applicationName);
}
