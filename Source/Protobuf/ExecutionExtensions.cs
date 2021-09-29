// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using ExecutionContextContract = Dolittle.Execution.Contracts.ExecutionContext;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents conversion extensions for the common execution types.
    /// </summary>
    public static class ExecutionExtensions
    {
        /// <summary>
        /// Convert a <see cref="ExecutionContext"/> to <see cref="ExecutionContextContract"/>.
        /// </summary>
        /// <param name="executionContext"><see cref="ExecutionContext"/> to convert from.</param>
        /// <returns>Converted <see cref="ExecutionContextContract"/>.</returns>
        public static ExecutionContextContract ToProtobuf(this ExecutionContext executionContext)
        {
            var message = new ExecutionContextContract
            {
                MicroserviceId = executionContext.Microservice.Value.ToProtobuf(),
                TenantId = executionContext.Tenant.Value.ToProtobuf(),
                CorrelationId = executionContext.CorrelationId.Value.ToProtobuf(),
                Environment = executionContext.Environment.Value,
                Version = executionContext.Version.ToProtobuf()
            };
            message.Claims.AddRange(executionContext.Claims.ToProtobuf());

            return message;
        }

        /// <summary>
        /// Convert a <see cref="ExecutionContextContract"/> to <see cref="ExecutionContext"/>.
        /// </summary>
        /// <param name="executionContext"><see cref="ExecutionContextContract"/> to convert from.</param>
        /// <returns>Converted <see cref="ExecutionContext"/>.</returns>
        public static ExecutionContext ToExecutionContext(this ExecutionContextContract executionContext) =>
            new(
                executionContext.MicroserviceId.ToGuid(),
                executionContext.TenantId.ToGuid(),
                executionContext.Version.ToVersion(),
                executionContext.Environment,
                executionContext.CorrelationId.ToGuid(),
                executionContext.Claims.ToClaims(),
                CultureInfo.InvariantCulture);
    }
}