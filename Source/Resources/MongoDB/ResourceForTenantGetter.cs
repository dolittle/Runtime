// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resources.Contracts;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Represents the implementation of <see cref="ICanGetResourceForTenant"/>.
/// </summary>
public class ResourceForTenantGetter : ICanGetResourceForTenant
{
    readonly Func<TenantId, IKnowTheConnectionString> _getConnectionString;
    readonly ILogger _logger;

        
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceForTenantGetter"/> class.
    /// </summary>
    /// <param name="getConnectionString">The factory to call to get the connection string for a specific tenant.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    public ResourceForTenantGetter(Func<TenantId, IKnowTheConnectionString> getConnectionString, ILogger logger)
    {
        _getConnectionString = getConnectionString;
        _logger = logger;
    }

    /// <inheritdoc />
    public GetMongoDBResponse GetResource(ExecutionContext executionContext)
    {
        try
        {
            _logger.GetResourceCalled(executionContext.Tenant);
                
            var mongoUrl = _getConnectionString(executionContext.Tenant).ConnectionString;
            return new GetMongoDBResponse
            {
                ConnectionString = mongoUrl.ToString(),
            };
        }
        catch (Exception ex)
        {
            _logger.FailedToGetResource(executionContext.Tenant, ex);
            return new GetMongoDBResponse {Failure = new Failure(ex.Message) };
        }
    }
}
