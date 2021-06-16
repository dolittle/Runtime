// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Events.Processing.Filters.Partitioned
{
    /// <summary>
    /// Represents an implementation of <see cref="IPartitionedFiltersProtocol" />.
    /// </summary>
    public class PartitionedFiltersProtocol : FiltersProtocol<PartitionedFilterClientToRuntimeMessage, PartitionedFilterRegistrationRequest, PartitionedFilterResponse, PartitionedFilterRegistrationArguments>, IPartitionedFiltersProtocol
    {
        /// <inheritdoc/>
        public override PartitionedFilterRegistrationArguments ConvertConnectArguments(PartitionedFilterRegistrationRequest arguments)
            => new(arguments.CallContext.ExecutionContext.ToExecutionContext(), arguments.FilterId.ToGuid(), arguments.ScopeId.ToGuid());

        /// <inheritdoc/>
        public override ReverseCallArgumentsContext GetArgumentsContext(PartitionedFilterRegistrationRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public override PartitionedFilterRegistrationRequest GetConnectArguments(PartitionedFilterClientToRuntimeMessage message)
            => message.RegistrationRequest;

        /// <inheritdoc/>
        public override Pong GetPong(PartitionedFilterClientToRuntimeMessage message)
            => message.Pong;

        /// <inheritdoc/>
        public override PartitionedFilterResponse GetResponse(PartitionedFilterClientToRuntimeMessage message)
            => message.FilterResult;

        /// <inheritdoc/>
        public override ReverseCallResponseContext GetResponseContext(PartitionedFilterResponse message)
            => message.CallContext;
    }
}
