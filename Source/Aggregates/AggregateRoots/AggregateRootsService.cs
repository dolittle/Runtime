// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Aggregates.Contracts.AggregateRoots;

namespace Dolittle.Runtime.Aggregates.AggregateRoots
{
    /// <summary>
    /// Represents the implementation of <see cref="AggregateRootsBase"/>.
    /// </summary>
    public class AggregateRootsService : AggregateRootsBase
    {
        readonly IAggregateRoots _aggregateRoots;
        readonly ILogger _logger;

        public AggregateRootsService(IAggregateRoots aggregateRoots, ILogger logger)
        {
            _aggregateRoots = aggregateRoots;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<AggregateRootRegistrationResponse> Register(
            AggregateRootRegistrationRequest request,
            ServerCallContext context)
        {
            _aggregateRoots.Register(request.HasAlias 
                ? new AggregateRoot(request.AggregateRoot.ToArtifact(), request.Alias) 
                : new AggregateRoot(request.AggregateRoot.ToArtifact()));
            return Task.FromResult(new AggregateRootRegistrationResponse());
        }
    }
}
