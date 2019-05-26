/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications;
using Dolittle.Collections;
using Dolittle.Tenancy;
using System.Globalization;
using System.Linq;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting <see cref="ExecutionContext"/> to and from protobuf representations
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Convert from <see cref="ExecutionContext"/> to <see cref="Dolittle.Execution.ExecutionContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="ExecutionContext"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Execution.ExecutionContext"/></returns>
        public static Dolittle.Execution.ExecutionContext ToExecutionContext(this ExecutionContext protobuf)
        {
            return new Dolittle.Execution.ExecutionContext(
                protobuf.Application.ToConcept<Application>(),
                protobuf.BoundedContext.ToConcept<BoundedContext>(),
                protobuf.Tenant.ToConcept<TenantId>(),
                protobuf.Environment,
                protobuf.CorrelationId.ToConcept<Dolittle.Execution.CorrelationId>(),
                protobuf.Claims.ToClaims(),
                CultureInfo.GetCultureInfo(protobuf.Culture)
            );
        }    

        /// <summary>
        /// Convert from <see cref="Dolittle.Execution.ExecutionContext"/> to <see cref="ExecutionContext"/>
        /// </summary>
        /// <param name="executionContext"><see cref="Dolittle.Execution.ExecutionContext"/> to convert from</param>
        /// <returns>Converted <see cref="ExecutionContext"/></returns>
        public static ExecutionContext ToProtobuf(this Dolittle.Execution.ExecutionContext executionContext)
        {
            var protobuf = new ExecutionContext
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