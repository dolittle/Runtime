// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Aggregates.Contracts.AggregateRoots;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents the implementation of <see cref="AggregateRootsBase"/>.
    /// </summary>
    public class AggregateRootsService : AggregateRootsBase
    {
        readonly IAggregateRoots _aggregateRoots;

        public AggregateRootsService(IAggregateRoots aggregateRoots)
        {
            _aggregateRoots = aggregateRoots;
        }

        /// <inheritdoc/>
        public override Task<AggregateRootAliasRegistrationResponse> RegisterAlias(
            AggregateRootAliasRegistrationRequest request,
            ServerCallContext context)
        {
            _aggregateRoots.Register(request.HasAlias 
                ? new AggregateRoot(request.AggregateRoot.ToAggregateRootId(), request.Alias) 
                : new AggregateRoot(request.AggregateRoot.ToAggregateRootId()));
            return Task.FromResult(new AggregateRootAliasRegistrationResponse());
        }
    }
}
