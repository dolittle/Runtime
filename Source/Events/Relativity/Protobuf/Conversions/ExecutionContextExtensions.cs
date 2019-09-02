/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion;
using Dolittle.Tenancy;
using System.Globalization;
using System.Linq;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting <see cref="ExecutionContext"/> to and from protobuf representations
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.OriginalContext"/> to <see cref="OriginalContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.OriginalContext"/> to convert from</param>
        /// <returns>Converted <see cref="OriginalContext"/></returns>
        public static OriginalContext ToOriginalContext(this Runtime.Grpc.Interaction.OriginalContext protobuf)
        {
            return new OriginalContext(
                protobuf.Application.ToConcept<Dolittle.Applications.Application>(),
                protobuf.BoundedContext.ToConcept<BoundedContext>(),
                protobuf.Tenant.ToConcept<TenantId>(),
                protobuf.Environment,
                protobuf.Claims.ToClaims(),
                protobuf.CommitInOrigin
            );
        }

        /// <summary>
        /// Convert from <see cref="OriginalContext"/> to <see cref="Runtime.Grpc.Interaction.OriginalContext"/>
        /// </summary>
        /// <param name="originalContext"></param>
        /// <returns></returns>
        public static Runtime.Grpc.Interaction.OriginalContext ToProtobuf(this OriginalContext originalContext)
        {
            var protobuf = new Runtime.Grpc.Interaction.OriginalContext 
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