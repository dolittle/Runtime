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
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.OriginalContext"/> to <see cref="OriginalContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.Protobuf.OriginalContext"/> to convert from</param>
        /// <returns>Converted <see cref="OriginalContext"/></returns>
        public static OriginalContext ToOriginalContext(this Runtime.Grpc.Interaction.Protobuf.OriginalContext protobuf)
        {
            return new OriginalContext(
                protobuf.Application.ToConcept<Application>(),
                protobuf.BoundedContext.ToConcept<BoundedContext>(),
                protobuf.Tenant.ToConcept<TenantId>(),
                protobuf.Environment,
                protobuf.Claims.ToClaims(),
                protobuf.CommitInOrigin
            );
        }

        /// <summary>
        /// Convert from <see cref="OriginalContext"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.OriginalContext"/>
        /// </summary>
        /// <param name="originalContext"></param>
        /// <returns></returns>
        public static Runtime.Grpc.Interaction.Protobuf.OriginalContext ToProtobuf(this OriginalContext originalContext)
        {
            var protobuf = new Runtime.Grpc.Interaction.Protobuf.OriginalContext 
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


        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.ExecutionContext"/> to <see cref="ExecutionContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="ExecutionContext"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Execution.ExecutionContext"/></returns>
        public static ExecutionContext ToExecutionContext(this Runtime.Grpc.Interaction.Protobuf.ExecutionContext protobuf)
        {
            return new ExecutionContext(
                protobuf.Application.ToConcept<Application>(),
                protobuf.BoundedContext.ToConcept<BoundedContext>(),
                protobuf.Tenant.ToConcept<TenantId>(),
                protobuf.Environment,
                protobuf.CorrelationId.ToConcept<CorrelationId>(),
                protobuf.Claims.ToClaims(),
                CultureInfo.GetCultureInfo(protobuf.Culture)
            );
        }    



        /// <summary>
        /// Convert from <see cref="ExecutionContext"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.ExecutionContext"/>
        /// </summary>
        /// <param name="executionContext"><see cref="ExecutionContext"/> to convert from</param>
        /// <returns>Converted <see cref="Runtime.Grpc.Interaction.Protobuf.ExecutionContext"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.ExecutionContext ToProtobuf(this ExecutionContext executionContext)
        {
            var protobuf = new Runtime.Grpc.Interaction.Protobuf.ExecutionContext
            {
                Application = executionContext.Application.ToProtobuf(),
                BoundedContext = executionContext.BoundedContext.ToProtobuf(),
                Tenant = executionContext.Tenant.ToProtobuf(),
                CorrelationId = executionContext.CorrelationId.ToProtobuf(),
                Environment = executionContext.Environment.Value,
                Culture = executionContext.Culture?.Name ?? CultureInfo.InvariantCulture.Name
            };
            executionContext.Claims.ToProtobuf().ForEach(protobuf.Claims.Add);
            return protobuf;
        }
    }
}