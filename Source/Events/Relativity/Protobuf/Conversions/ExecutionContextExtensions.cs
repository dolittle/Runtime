/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using Dolittle.Applications;
using Dolittle.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Tenancy;
using Dolittle.Protobuf;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting <see cref="ExecutionContext"/> to and from protobuf representations
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Convert from <see cref="Dolittle.Events.Relativity.Microservice.OriginalContext"/> to <see cref="OriginalContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Dolittle.Events.Relativity.Microservice.OriginalContext"/> to convert from</param>
        /// <returns>Converted <see cref="OriginalContext"/></returns>
        public static OriginalContext ToOriginalContext(this Dolittle.Events.Relativity.Microservice.OriginalContext protobuf)
        {
            return new OriginalContext(
                protobuf.Application.To<Dolittle.Applications.Application>(),
                protobuf.BoundedContext.To<BoundedContext>(),
                protobuf.Tenant.To<TenantId>(),
                protobuf.Environment,
                protobuf.Claims.ToClaims(),
                protobuf.CommitInOrigin
            );
        }

        /// <summary>
        /// Convert from <see cref="OriginalContext"/> to <see cref="Dolittle.Events.Relativity.Microservice.OriginalContext"/>
        /// </summary>
        /// <param name="originalContext"></param>
        /// <returns></returns>
        public static Dolittle.Events.Relativity.Microservice.OriginalContext ToProtobuf(this OriginalContext originalContext)
        {
            var protobuf = new Dolittle.Events.Relativity.Microservice.OriginalContext 
            {
                Application = originalContext.Application.ToProtobuf(),
                Tenant = originalContext.Tenant.ToProtobuf(),
                BoundedContext = originalContext.BoundedContext.ToProtobuf(),
                Environment = originalContext.Environment.Value,
                CommitInOrigin = originalContext.CommitInOrigin.Value
            };
            protobuf.Claims.AddRange(originalContext.Claims.Select(c => c.ToProtobuf()));
            return protobuf;
        }
    }
}