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
        /// <returns>Converted <see cref="Dolittle.Events.Relativity.Microservice.TenantOffset"/></returns>
        public static Dolittle.Events.Relativity.Microservice.TenantOffset ToProtobuf(this TenantOffset tenantOffset)
        {
            var message = new Dolittle.Events.Relativity.Microservice.TenantOffset();
            message.Tenant = tenantOffset.Tenant.ToProtobuf();
            message.Offset = tenantOffset.Offset;
            return message;
        }

        /// <summary>
        /// Convert from <see cref="Dolittle.Events.Relativity.Microservice.TenantOffset"/> to <see cref="TenantOffset"/>
        /// </summary>
        /// <param name="tenantOffset"><see cref="Dolittle.Events.Relativity.Microservice.TenantOffset"/> to convert from</param>
        /// <returns>Converted <see cref="TenantOffset"/></returns>
        public static TenantOffset ToTenantOffset(this Dolittle.Events.Relativity.Microservice.TenantOffset tenantOffset)
        {
            return new TenantOffset(
                tenantOffset.Tenant.ToConcept<TenantId>(), 
                tenantOffset.Offset);
        }

        /// <summary>
        /// Convert from collection of <see cref="TenantOffset"/> to collection of <see cref="Dolittle.Events.Relativity.Microservice.TenantOffset"/>
        /// </summary>
        /// <param name="offsets">Collection of <see cref="TenantOffset">Offsets</see> to convert from</param>
        /// <returns>Collection of <see cref="Dolittle.Events.Relativity.Microservice.TenantOffset"/></returns>
        public static RepeatedField<Dolittle.Events.Relativity.Microservice.TenantOffset> ToProtobuf(this IEnumerable<TenantOffset> offsets)
        {
            var protobuf = new RepeatedField<Dolittle.Events.Relativity.Microservice.TenantOffset>();
            protobuf.Add(offsets.Select(_ => _.ToProtobuf()));
            return protobuf;
        }     


        /// <summary>
        /// Convert from  collection of <see cref="Dolittle.Events.Relativity.Microservice.TenantOffset"/> to collection of <see cref="TenantOffset"/>
        /// </summary>
        /// <param name="offsets"><see cref="Dolittle.Events.Relativity.Microservice.TenantOffset">Offsets</see> to convert from</param>
        /// <returns>Converted <see cref="TenantOffset">offsets</see></returns>
        public static IEnumerable<TenantOffset> ToTenantOffsets(this IEnumerable<Dolittle.Events.Relativity.Microservice.TenantOffset> offsets)
        {
            return offsets.Select(_ => _.ToTenantOffset()).ToArray();
        }     
    }
}