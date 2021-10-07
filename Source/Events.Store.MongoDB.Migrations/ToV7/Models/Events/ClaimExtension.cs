// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7.Models.Events
{
    /// <summary>
    /// Extension methods for <see cref="Claim" />.
    /// </summary>
    public static class ClaimExtension
    {
        /// <summary>
        /// Converts <see cref="Claim" /> to <see cref="Security.Claim" />.
        /// </summary>
        /// <param name="claim"><see cref="Claim" />.</param>
        /// <returns>Converted <see cref="Security.Claim" />.</returns>
        public static Security.Claim ToClaim(this Claim claim) =>
            new Security.Claim(claim.Name, claim.Value, claim.ValueType);

        /// <summary>
        /// Converts <see cref="IEnumerable{T}" /> of <see cref="Claim" /> to <see cref="Security.Claims" />.
        /// </summary>
        /// <param name="claims">The <see cref="IEnumerable{T}" /> of <see cref="Claim" />.</param>
        /// <returns>Converted <see cref="Security.Claims" />.</returns>
        public static Security.Claims ToClaims(this IEnumerable<Claim> claims) =>
            new Security.Claims(claims.Select(_ => _.ToClaim()));

        /// <summary>
        /// Converts <see cref="Security.Claim" /> to <see cref="Claim" />.
        /// </summary>
        /// <param name="claim"><see cref="Security.Claim" />.</param>
        /// <returns>Converted <see cref="Claim" />.</returns>
        public static Claim ToStoreRepresentation(this Security.Claim claim) =>
            new Claim(claim.Name, claim.Value, claim.ValueType);

        /// <summary>
        /// Converts <see cref="Security.Claims" /> to <see cref="IEnumerable{T}" /> of <see cref="Claim" />.
        /// </summary>
        /// <param name="claims"><see cref="Security.Claims" />.</param>
        /// <returns>Converted <see cref="IEnumerable{T}" /> of <see cref="Claim" />.</returns>
        public static IEnumerable<Claim> ToStoreRepresentation(this Security.Claims claims) =>
            claims.Select(ToStoreRepresentation);
    }
}
