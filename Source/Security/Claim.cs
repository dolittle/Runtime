// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a Claim.
    /// </summary>
    public record Claim(string Name, string Value, string ValueType)
    {
        /// <summary>
        /// Converts the <see cref="System.Security.Claims.Claim" /> instance into the corresponding <see cref="Claim" /> instance.
        /// </summary>
        /// <param name="claim"><see cref="System.Security.Claims.Claim"/> to convert.</param>
        /// <returns>a <see cref="Claim" /> instance.</returns>
        public static Claim FromDotnetClaim(System.Security.Claims.Claim claim)
        {
            if (claim == null)
                return null;

            return new Claim(claim.Type, claim.Value, claim.ValueType);
        }

        /// <summary>
        /// Converts the <see cref="Claim" /> instance into the corresponding <see cref="System.Security.Claims.Claim" /> instance.
        /// </summary>
        /// <returns>a <see cref="System.Security.Claims.Claim" /> instance.</returns>
        public System.Security.Claims.Claim ToDotnetClaim() => new(Name, Value, ValueType);
    }
}