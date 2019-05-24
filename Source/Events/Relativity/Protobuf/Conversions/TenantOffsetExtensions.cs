/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Tenancy;
using Google.Protobuf.Collections;
using Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion;

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
        /// <returns>Converted <see cref="Runtime.Grpc.Interaction.Protobuf.TenantOffset"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.TenantOffset ToProtobuf(this TenantOffset tenantOffset)
        {
            var message = new Runtime.Grpc.Interaction.Protobuf.TenantOffset();
            message.Tenant = tenantOffset.Tenant.ToProtobuf();
            message.Offset = tenantOffset.Offset;
            return message;
        }

        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.TenantOffset"/> to <see cref="TenantOffset"/>
        /// </summary>
        /// <param name="tenantOffset"><see cref="Runtime.Grpc.Interaction.Protobuf.TenantOffset"/> to convert from</param>
        /// <returns>Converted <see cref="TenantOffset"/></returns>
        public static TenantOffset ToTenantOffset(this Runtime.Grpc.Interaction.Protobuf.TenantOffset tenantOffset)
        {
            return new TenantOffset(
                tenantOffset.Tenant.ToConcept<TenantId>(), 
                tenantOffset.Offset);
        }

        /// <summary>
        /// Convert from collection of <see cref="TenantOffset"/> to collection of <see cref="Runtime.Grpc.Interaction.Protobuf.TenantOffset"/>
        /// </summary>
        /// <param name="offsets">Collection of <see cref="TenantOffset">Offsets</see> to convert from</param>
        /// <returns>Collection of <see cref="Runtime.Grpc.Interaction.Protobuf.TenantOffset"/></returns>
        public static RepeatedField<Runtime.Grpc.Interaction.Protobuf.TenantOffset> ToProtobuf(this IEnumerable<TenantOffset> offsets)
        {
            var protobuf = new RepeatedField<Runtime.Grpc.Interaction.Protobuf.TenantOffset>();
            protobuf.Add(offsets.Select(_ => _.ToProtobuf()));
            return protobuf;
        }     


        /// <summary>
        /// Convert from <see cref="IEnumerable{TenantOffset}"/> to collection of <see cref="Dolittle.Runtime.Events.Relativity.TenantOffset"/>
        /// </summary>
        /// <param name="offsets"><see cref="TenantOffset">Offsets</see> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Runtime.Events.Relativity.TenantOffset">offsets</see></returns>
        public static IEnumerable<TenantOffset> ToTenantOffsets(this IEnumerable<Runtime.Grpc.Interaction.Protobuf.TenantOffset> offsets)
        {
            return offsets.Select(_ => _.ToTenantOffset()).ToArray();
        }     
    }
}