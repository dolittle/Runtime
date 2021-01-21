// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a Claim.
    /// </summary>
    public class Claim : Value<Claim>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Claim"/> class.
        /// </summary>
        /// <param name="name">The Name of the claim.</param>
        /// <param name="value">The Value of the claim.</param>
        /// <param name="valueType">The type of the Value of the claim.</param>
        public Claim(string name, string value, string valueType)
        {
            Name = name;
            Value = value;
            ValueType = valueType;
        }

        /// <summary>
        /// Gets the name of the claim.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the claim.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the type of the value of the claim.
        /// </summary>
        public string ValueType { get; }

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
        public System.Security.Claims.Claim ToDotnetClaim()
        {
            return new System.Security.Claims.Claim(Name, Value, ValueType);
        }
    }
}