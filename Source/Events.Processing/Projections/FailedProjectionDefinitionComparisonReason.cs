// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the reason for why the comparison of a <see cref="ProjectionDefintion" /> failed.
/// </summary>
public record FailedProjectionDefinitionComparisonReason(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts <see cref="string" /> to <see cref="FailedProjectionDefinitionComparisonReason" />.
    /// </summary>
    /// <param name="reason">The value.</param>
    public static implicit operator FailedProjectionDefinitionComparisonReason(string reason) => new(reason);
}