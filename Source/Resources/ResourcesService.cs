// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resources.Contracts;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using static Dolittle.Runtime.Resources.Contracts.Resources;

namespace Dolittle.Runtime.Resources;

/// <summary>
/// Represents an implementation of <see cref="ResourcesBase"/>.
/// </summary>
[PrivateService]
public class ResourcesService : ResourcesBase
{
    readonly MongoDB.ICanGetResourceForTenant _mongodb;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourcesService"/> class.
    /// </summary>
    public ResourcesService(MongoDB.ICanGetResourceForTenant mongodbService)
    {
        _mongodb = mongodbService;
    }

    /// <inheritdoc />
    public override Task<GetMongoDBResponse> GetMongoDB(GetRequest request, ServerCallContext context)
        => Task.FromResult(_mongodb.GetResource(request.CallContext.ExecutionContext.ToExecutionContext()));
}
