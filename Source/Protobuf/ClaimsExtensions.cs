// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Google.Protobuf.Collections;
using grpc = Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Extensions for converting back and forth with <see cref="Security.Claim"/>.
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Convert from <see cref="RepeatedField{T}"/> of <see cref="grpc.Claim"/> to <see cref="Security.Claims"/>.
        /// </summary>
        /// <param name="claims"><see cref="RepeatedField{T}"/> of <see cref="grpc.Claim"/> to convert from.</param>
        /// <returns>Converted <see cref="Security.Claims"/>.</returns>
        public static Security.Claims ToClaims(this RepeatedField<grpc.Claim> claims)
        {
            return new Security.Claims(claims.Select(_ => _.ToClaim()).ToArray());
        }

        /// <summary>
        /// Convert from <see cref="Security.Claims"/> to <see cref="RepeatedField{T}"/> of <see cref="grpc.Claim"/>.
        /// </summary>
        /// /// <param name="claims"><see cref="Security.Claims"/> to convert from.</param>
        /// <returns>Converted <see cref="RepeatedField{T}"/> of <see cref="grpc.Claim"/>.</returns>
        public static RepeatedField<grpc.Claim> ToProtobuf(this Security.Claims claims)
        {
            var protobufClaims = new RepeatedField<grpc.Claim>
            {
                claims.Select(_ => _.ToProtobuf())
            };
            return protobufClaims;
        }

        /// <summary>
        /// Convert from <see cref="Security.Claim"/> to <see cref="grpc.Claim"/>.
        /// </summary>
        /// <param name="claim"><see cref="Security.Claim"/> to convert from.</param>
        /// <returns>Converted <see cref="grpc.Claim"/>.</returns>
        public static grpc.Claim ToProtobuf(this Security.Claim claim)
        {
            return new grpc.Claim
            {
                Name = claim.Name,
                Value = claim.Value,
                ValueType = claim.ValueType
            };
        }

        /// <summary>
        /// Convert from <see cref="grpc.Claim"/> to <see cref="Security.Claim"/>.
        /// </summary>
        /// <param name="claim"><see cref="grpc.Claim"/> to convert from.</param>
        /// <returns>Converted <see cref="Security.Claim"/>.</returns>
        public static Security.Claim ToClaim(this grpc.Claim claim)
        {
            return new Security.Claim(claim.Name, claim.Value, claim.ValueType);
        }
    }
}