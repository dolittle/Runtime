// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a set of <see cref="Claim">Claims</see>.
    /// </summary>
    public class Claims : IEnumerable<Claim>, IEquatable<Claims>
    {
        /// <summary>
        /// Gets the empty representation of <see cref="Claims"/>.
        /// </summary>
        public static readonly Claims Empty = new(Array.Empty<Claim>());

        readonly List<Claim> _claims = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Claims"/> class.
        /// </summary>
        /// <param name="claims">The claims to populate.</param>
        public Claims(IEnumerable<Claim> claims)
        {
            _claims.AddRange(claims ?? Enumerable.Empty<Claim>());
        }

        /// <summary>
        /// Uses the same equality as the Equals method.
        /// </summary>
        /// <param name="leftHandSide">Left hand side <see cref="Claim"/>.</param>
        /// <param name="rightHandSide">Right hand side <see cref="Claim"/>.</param>
        /// <returns>True if equals, false otherwise.</returns>
        public static bool operator ==(Claims leftHandSide, Claims rightHandSide)
        {
            if (object.Equals(leftHandSide, null) && object.Equals(rightHandSide, null))
            {
                return true;
            }

            return Equals(leftHandSide, null) ? false : leftHandSide.Equals(rightHandSide);
        }

        /// <summary>
        /// Uses the same equality as the Equals method.
        /// </summary>
        /// <param name="leftHandSide">Left hand side <see cref="Claim"/>.</param>
        /// <param name="rightHandSide">Right hand side <see cref="Claim"/>.</param>
        /// <returns>true if not equals, false otherwise.</returns>
        public static bool operator !=(Claims leftHandSide, Claims rightHandSide) => !(leftHandSide == rightHandSide);

        /// <inheritdoc/>
        public bool Equals(Claims other)
        {
            if (other == null || other.Count() != this.Count())
                return false;

            var thisClaims = _claims.OrderBy(_ => _.Name).ThenBy(_ => _.ValueType).ThenBy(_ => _.Value).ToArray();
            var otherClaims = other.OrderBy(_ => _.Name).ThenBy(_ => _.ValueType).ThenBy(_ => _.Value).ToArray();

            for (var i = 0; i < thisClaims.Length; i++)
            {
                if (!object.Equals(thisClaims[i], otherClaims[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object other) => Equals(other as Claims);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var array = _claims.OrderBy(_ => _.Name).ThenBy(_ => _.ValueType).ThenBy(_ => _.Value).ToArray();
            return HashCodeHelper.GetHashCode(array);
        }

        /// <summary>
        /// Gets an enumerator to iterate over the claims.
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> of <see cref="Claim"/>.</returns>
        public IEnumerator<Claim> GetEnumerator() => _claims.GetEnumerator();

        /// <summary>
        /// Gets an enumerator to iterate over the claims.
        /// </summary>
        /// <returns><see cref="IEnumerator"/> of <see cref="Claim"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _claims.GetEnumerator();
    }
}
