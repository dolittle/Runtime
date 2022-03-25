// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Execution;
using ClaimContracts = Dolittle.Security.Contracts.Claim;

namespace Dolittle.Runtime.Protobuf;

/// <summary>
/// Represents conversion extensions for the common security types.
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Convert from <see cref="Claims"/> to <see cref="IEnumerable{T}"/> of <see cref="ClaimContracts"/>.
    /// </summary>
    /// <param name="claims"><see cref="Claims"/> to convert from.</param>
    /// <returns><see cref="IEnumerable{T}"/> of <see cref="ClaimContracts"/>.</returns>
    public static IEnumerable<ClaimContracts> ToProtobuf(this Claims claims) =>
        claims.Select(_ => new ClaimContracts
        {
            Key = _.Name,
            Value = _.Value,
            ValueType = _.ValueType
        });

    /// <summary>
    /// Convert from <see cref="IEnumerable{T}"/> of <see cref="ClaimContracts"/> to <see cref="Claims"/>.
    /// </summary>
    /// <param name="claims"><see cref="IEnumerable{T}"/> of <see cref="ClaimContracts"/> to convert from.</param>
    /// <returns>Converted <see cref="Claims"/>.</returns>
    public static Claims ToClaims(this IEnumerable<ClaimContracts> claims) =>
        new(claims.Select(_ => new Claim(_.Key, _.Value, _.ValueType)));
}