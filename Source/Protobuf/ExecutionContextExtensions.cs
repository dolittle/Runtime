// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Applications;
using Dolittle.Collections;
using Dolittle.Protobuf;
using Dolittle.Tenancy;
using grpc = Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Extensions for converting <see cref="grpc.ExecutionContext"/> to and from protobuf representations.
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Convert from <see cref="grpc.ExecutionContext"/> to <see cref="Execution.ExecutionContext"/>.
        /// </summary>
        /// <param name="protobuf"><see cref="grpc.ExecutionContext"/> to convert from.</param>
        /// <returns>Converted <see cref="Execution.ExecutionContext"/>.</returns>
        public static Execution.ExecutionContext ToExecutionContext(this grpc.ExecutionContext protobuf)
        {
            return new Execution.ExecutionContext(
                protobuf.Application.To<Application>(),
                protobuf.BoundedContext.To<BoundedContext>(),
                protobuf.Tenant.To<TenantId>(),
                protobuf.Environment,
                protobuf.CorrelationId.To<Dolittle.Execution.CorrelationId>(),
                protobuf.Claims.ToClaims(),
                CultureInfo.GetCultureInfo(protobuf.Culture));
        }

        /// <summary>
        /// Convert from <see cref="Execution.ExecutionContext"/> to <see cref="grpc.ExecutionContext"/>.
        /// </summary>
        /// <param name="executionContext"><see cref="Execution.ExecutionContext"/> to convert from.</param>
        /// <returns>Converted <see cref="grpc.ExecutionContext"/>.</returns>
        public static grpc.ExecutionContext ToProtobuf(this Execution.ExecutionContext executionContext)
        {
            var protobuf = new grpc.ExecutionContext
            {
                Application = Extensions.ToProtobuf(executionContext.Application),
                BoundedContext = Extensions.ToProtobuf(executionContext.BoundedContext),
                Tenant = Extensions.ToProtobuf(executionContext.Tenant),
                CorrelationId = Extensions.ToProtobuf(executionContext.CorrelationId),
                Environment = executionContext.Environment.Value,
                Culture = executionContext.Culture?.Name ?? CultureInfo.InvariantCulture.Name
            };
            executionContext.Claims.ToProtobuf().ForEach(protobuf.Claims.Add);
            return protobuf;
        }
    }
}