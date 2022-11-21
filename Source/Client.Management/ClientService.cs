// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Client.Management.Contracts;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client.Management;

/// <summary>
/// Represents an implementation of <see cref="Contracts.Client.ClientBase"/>.
/// </summary>
[ManagementService, ManagementWebService]
public class ClientService : Contracts.Client.ClientBase
{
    readonly IBuildResultsForHeads _buildResults;
    readonly ILogger _logger;
        
    public ClientService(
        IBuildResultsForHeads buildResults,
        ILogger logger)
    {
        _buildResults = buildResults;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public override Task<GetBuildResultsResponse> GetBuildResults(GetBuildResultsRequest request, ServerCallContext context)
    {
        _logger.GettingClientBuildResults();
        var response = new GetBuildResultsResponse
        {
            BuildResults = _buildResults.GetFor(Guid.Empty).ToProtobuf()
        };
        return Task.FromResult(response);
    }
}
