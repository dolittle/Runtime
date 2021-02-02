// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using ExecutionContextContract = Dolittle.Execution.Contracts.ExecutionContext;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Defines extension on top of <see cref="IExecutionContextManager"/>.
    /// </summary>
    public static class ExecutionContextManagerExtensions
    {
        /// <summary>
        /// Set current execution context for a Protobuf <see cref="ExecutionContextContract"/>.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> to extend.</param>
        /// <param name="executionContext"><see cref="ExecutionContextContract"/> to set current.</param>
        public static void CurrentFor(this IExecutionContextManager executionContextManager, ExecutionContextContract executionContext)
        {
            var microservice = executionContext.MicroserviceId.ToGuid();
            var tenant = executionContext.TenantId.ToGuid();
            var correlationId = executionContext.CorrelationId.ToGuid();
            var claims = executionContext.Claims.ToClaims();

            executionContextManager.CurrentFor(
                microservice,
                tenant,
                correlationId,
                claims);
        }
    }
}