// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resources.Contracts;
using Dolittle.Runtime.Resources.MongoDB;
using Grpc.Core;
using static Dolittle.Runtime.Resources.Contracts.Resources;

namespace Dolittle.Runtime.Resources
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class ResourcesService : ResourcesBase
    {
        readonly IService _mongodbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesService"/> class.
        /// </summary>
        public ResourcesService(MongoDB.IService mongodbService)
        {
            _mongodbService = mongodbService;
        }

        /// <inheritdoc />
        public override Task<GetMongoDbResponse> GetMongoDb(GetRequest request, ServerCallContext context)
            => Task.FromResult(_mongodbService.GetResource(request.CallContext.ExecutionContext.ToExecutionContext()));
    }
}
