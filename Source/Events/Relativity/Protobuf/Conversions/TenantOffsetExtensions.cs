/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Tenancy;
using Google.Protobuf.Collections;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting <see cref="Dolittle.Runtime.Events.Relativity.TenantOffset"/> to and from protobuf representations
    /// </summary>
    public static class TenantOffsetExtensions
    {
        /// <summary>
        /// Convert from <see cref="TenantOffset"/> to <see cref="TenantOffset"/>
        /// </summary>
        /// /// <param name="tenantOffset"><see cref="TenantOffset"/> to convert from</param>
        /// <returns>Converted <see cref="Interaction.Grpc.TenantOffset"/></returns>
        public static Interaction.Grpc.TenantOffset ToProtobuf(this TenantOffset tenantOffset)
        {
            var message = new Interaction.Grpc.TenantOffset();
            message.Tenant = tenantOffset.Tenant.ToProtobuf();
            message.Offset = tenantOffset.Offset;
            return message;
        }

        /// <summary>
        /// Convert from <see cref="Interaction.Grpc.TenantOffset"/> to <see cref="TenantOffset"/>
        /// </summary>
        /// <param name="tenantOffset"><see cref="Interaction.Grpc.TenantOffset"/> to convert from</param>
        /// <returns>Converted <see cref="TenantOffset"/></returns>
        public static TenantOffset ToTenantOffset(this Interaction.Grpc.TenantOffset tenantOffset)
        {
            return new TenantOffset(
                tenantOffset.Tenant.ToConcept<TenantId>(), 
                tenantOffset.Offset);
        }

        /// <summary>
        /// Convert from collection of <see cref="TenantOffset"/> to collection of <see cref="Interaction.Grpc.TenantOffset"/>
        /// </summary>
        /// <param name="offsets">Collection of <see cref="TenantOffset">Offsets</see> to convert from</param>
        /// <returns>Collection of <see cref="Interaction.Grpc.TenantOffset"/></returns>
        public static RepeatedField<Interaction.Grpc.TenantOffset> ToProtobuf(this IEnumerable<TenantOffset> offsets)
        {
            var protobuf = new RepeatedField<Interaction.Grpc.TenantOffset>();
            protobuf.Add(offsets.Select(_ => _.ToProtobuf()));
            return protobuf;
        }     


        /// <summary>
        /// Convert from  collection of <see cref="Interaction.Grpc.TenantOffset"/> to collection of <see cref="TenantOffset"/>
        /// </summary>
        /// <param name="offsets"><see cref="Interaction.Grpc.TenantOffset">Offsets</see> to convert from</param>
        /// <returns>Converted <see cref="TenantOffset">offsets</see></returns>
        public static IEnumerable<TenantOffset> ToTenantOffsets(this IEnumerable<Interaction.Grpc.TenantOffset> offsets)
        {
            return offsets.Select(_ => _.ToTenantOffset()).ToArray();
        }     
    }
}