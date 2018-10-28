/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting back and forth with <see cref="Dolittle.Security.Claim"/>
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Convert from <see cref="RepeatedField{Claim}"/> to <see cref="Dolittle.Security.Claims"/>
        /// </summary>
        /// <param name="claims"><see cref="RepeatedField{Claim}"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Security.Claims"/></returns>
        public static Dolittle.Security.Claims ToClaims(this RepeatedField<Claim> claims)
        {
            return new Dolittle.Security.Claims(claims.Select(_ => _.ToClaim()).ToArray());
        }  

        /// <summary>
        /// Convert from <see cref="Dolittle.Security.Claims"/> to <see cref="RepeatedField{Claim}"/>
        /// </summary>
        /// <param name="claims"><see cref="Dolittle.Security.Claims"/> to convert from</param>
        /// <returns>Converted <see cref="RepeatedField{Claim}"/></returns>
        public static RepeatedField<Claim> ToProtobuf(this Dolittle.Security.Claims claims)
        {
            var protobufClaims = new RepeatedField<Claim>();
            protobufClaims.Add(claims.Select(_ => _.ToProtobuf()));
            return protobufClaims;
        } 

        /// <summary>
        /// Convert from <see cref="Dolittle.Security.Claim"/> to <see cref="Claim"/>
        /// </summary>
        /// <param name="claim"><see cref="Dolittle.Security.Claim"/> to convert from</param>
        /// <returns>Converted <see cref="Claim"/></returns>
        public static Claim ToProtobuf(this Dolittle.Security.Claim claim)
        {
            return new Claim {
                Name = claim.Name,
                Value = claim.Value,
                ValueType = claim.ValueType
            };
        }

        /// <summary>
        /// Convert from <see cref="Claim"/> to <see cref="Dolittle.Security.Claim"/>
        /// </summary>
        /// <param name="claim"><see cref="Claim"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Security.Claim"/></returns>
        public static Dolittle.Security.Claim ToClaim(this Claim claim)
        {
            return new Dolittle.Security.Claim(claim.Name, claim.Value, claim.ValueType);
        }
    }
}