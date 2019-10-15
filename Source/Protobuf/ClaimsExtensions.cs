/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using Dolittle.Events.Relativity.Microservice;
using Google.Protobuf.Collections;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Extensions for converting back and forth with <see cref="Security.Claim"/>
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Convert from <see cref="RepeatedField{Claim}"/> to <see cref="Security.Claims"/>
        /// </summary>
        /// <param name="claims"><see cref="RepeatedField{Claim}"/> to convert from</param>
        /// <returns>Converted <see cref="Security.Claims"/></returns>
        public static Security.Claims ToClaims(this RepeatedField<Claim> claims)
        {
            return new Security.Claims(claims.Select(_ => _.ToClaim()).ToArray());
        }  

        /// <summary>
        /// Convert from <see cref="Security.Claims"/> to <see cref="RepeatedField{Claim}"/>
        /// </summary>
        /// <param name="claims"><see cref="Security.Claims"/> to convert from</param>
        /// <returns>Converted <see cref="RepeatedField{Claim}"/></returns>
        public static RepeatedField<Claim> ToProtobuf(this Security.Claims claims)
        {
            var protobufClaims = new RepeatedField<Claim>
            {
                claims.Select(_ => _.ToProtobuf())
            };
            return protobufClaims;
        } 

        /// <summary>
        /// Convert from <see cref="Security.Claim"/> to <see cref="Claim"/>
        /// </summary>
        /// <param name="claim"><see cref="Security.Claim"/> to convert from</param>
        /// <returns>Converted <see cref="Claim"/></returns>
        public static Claim ToProtobuf(this Security.Claim claim)
        {
            return new Claim {
                Name = claim.Name,
                Value = claim.Value,
                ValueType = claim.ValueType
            };
        }

        /// <summary>
        /// Convert from <see cref="Claim"/> to <see cref="Security.Claim"/>
        /// </summary>
        /// <param name="claim"><see cref="Claim"/> to convert from</param>
        /// <returns>Converted <see cref="Security.Claim"/></returns>
        public static Security.Claim ToClaim(this Claim claim)
        {
            return new Security.Claim(claim.Name, claim.Value, claim.ValueType);
        }
    }
}