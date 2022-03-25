// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;

namespace Dolittle.Runtime.Services.HealthChecks;

/// <summary>
/// Represents an implementation of <see cref="Grpc.Health.V1.Health.HealthBase"/> that always returns healthy.
/// </summary>
public class HealthService : Grpc.Health.V1.Health.HealthBase
{
    public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        // TODO: Perhaps we need to do something clever here?
        return Task.FromResult(new HealthCheckResponse
        {
            Status = HealthCheckResponse.Types.ServingStatus.Serving,
        });
    }
    
    // TODO: I don't think we care - but there is also a Watch that we can implement
}
