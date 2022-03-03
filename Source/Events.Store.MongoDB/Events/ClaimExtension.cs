// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Extension methods for <see cref="Claim" />.
/// </summary>
public static class ClaimExtension
{
    /// <summary>
    /// Converts <see cref="Claim" /> to <see cref="Execution.Claim" />.
    /// </summary>
    /// <param name="claim"><see cref="Claim" />.</param>
    /// <returns>Converted <see cref="Execution.Claim" />.</returns>
    public static Execution.Claim ToClaim(this Claim claim) =>
        new(claim.Name, claim.Value, claim.ValueType);

    /// <summary>
    /// Converts <see cref="IEnumerable{T}" /> of <see cref="Claim" /> to <see cref="Claims" />.
    /// </summary>
    /// <param name="claims">The <see cref="IEnumerable{T}" /> of <see cref="Claim" />.</param>
    /// <returns>Converted <see cref="Claims" />.</returns>
    public static Execution.Claims ToClaims(this IEnumerable<Claim> claims) =>
        new(claims.Select(_ => _.ToClaim()));

    /// <summary>
    /// Converts <see cref="Execution.Claim" /> to <see cref="Claim" />.
    /// </summary>
    /// <param name="claim"><see cref="Execution.Claim" />.</param>
    /// <returns>Converted <see cref="Claim" />.</returns>
    public static Claim ToStoreRepresentation(this Execution.Claim claim) =>
        new(claim.Name, claim.Value, claim.ValueType);

    /// <summary>
    /// Converts <see cref="Claims" /> to <see cref="IEnumerable{T}" /> of <see cref="Claim" />.
    /// </summary>
    /// <param name="claims"><see cref="Claims" />.</param>
    /// <returns>Converted <see cref="IEnumerable{T}" /> of <see cref="Claim" />.</returns>
    public static IEnumerable<Claim> ToStoreRepresentation(this Execution.Claims claims) =>
        claims.Select(ToStoreRepresentation);
}
