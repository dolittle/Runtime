// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resources.Contracts;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Represents the implementation of <see cref="ICanGetResourceForTenant"/>.
    /// </summary>
    public class ResourceForTenantGetter : ICanGetResourceForTenant
    {
        readonly FactoryFor<IKnowTheConnectionString> _getConnectionString;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceForTenantGetter"/> class.
        /// </summary>
        /// <param name="getConnectionString">The <see cref="FactoryFor{T}"/> of type <see cref="IKnowTheConnectionString"/> to use to get connection strings after setting the execution context.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to use to set the execution context.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public ResourceForTenantGetter(FactoryFor<IKnowTheConnectionString> getConnectionString, IExecutionContextManager executionContextManager, ILogger logger)
        {
            _getConnectionString = getConnectionString;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc />
        public GetMongoDBResponse GetResource(ExecutionContext executionContext)
        {
            _logger.LogDebug("Getting MongoDB resource for tenant {Tenant}", executionContext.Tenant.Value);
            try
            {
                _executionContextManager.CurrentFor(executionContext);
                
                var mongoUrl = _getConnectionString().ConnectionString;
                return new GetMongoDBResponse
                {
                    ConnectionString = mongoUrl.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get MongoDb resource for tenant {Tenant}", executionContext.Tenant.Value);
                return new GetMongoDBResponse {Failure = new Failure(ex.Message) };
            }
        }
    }
}
