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
    /// Represents the implementation of <see cref="IService"/>.
    /// </summary>
    public class Service : IService
    {
        readonly FactoryFor<IResource> _getMongoDbResource;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        public Service(FactoryFor<IResource> getMongoDbResource, IExecutionContextManager executionContextManager, ILogger logger)
        {
            _getMongoDbResource = getMongoDbResource;
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
                return new GetMongoDBResponse(new GetMongoDBResponse
                {
                    ConnectionString = _getMongoDbResource().GetConnectionString().ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get MongoDb resource for tenant {Tenant}", executionContext.Tenant.Value);
                return new GetMongoDBResponse {Failure = new Failure(ex.Message) };
            }
        }
    }
}
