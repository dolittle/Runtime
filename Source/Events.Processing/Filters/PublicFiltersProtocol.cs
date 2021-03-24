// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    public record PublicFilterRegistrationArguments(ExecutionContext ExecutionContext, EventProcessorId Filter);
    /// <summary>
    /// Defines the protocol for public filters.
    /// </summary>
    public interface IPublicFiltersProtocol : IFiltersProtocol<PublicFilterClientToRuntimeMessage, PublicFilterRegistrationRequest, PartitionedFilterResponse, PublicFilterRegistrationArguments>
    {
    }

    /// <summary>
    /// Represents an implementation of <see cref="IPartitionedFiltersProtocol" />.
    /// </summary>
    public class PublicFiltersProtocol : FiltersProtocol<PublicFilterClientToRuntimeMessage, PublicFilterRegistrationRequest, PartitionedFilterResponse, PublicFilterRegistrationArguments>, IPublicFiltersProtocol
    {
        /// <inheritdoc/>
        public override PublicFilterRegistrationArguments ConvertConnectArguments(PublicFilterRegistrationRequest arguments)
            => new(arguments.CallContext.ExecutionContext.ToExecutionContext(), arguments.FilterId.ToGuid());

        /// <inheritdoc/>
        public override ReverseCallArgumentsContext GetArgumentsContext(PublicFilterRegistrationRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public override PublicFilterRegistrationRequest GetConnectArguments(PublicFilterClientToRuntimeMessage message)
            => message.RegistrationRequest;

        /// <inheritdoc/>
        public override Pong GetPong(PublicFilterClientToRuntimeMessage message)
            => message.Pong;

        /// <inheritdoc/>
        public override PartitionedFilterResponse GetResponse(PublicFilterClientToRuntimeMessage message)
            => message.FilterResult;

        /// <inheritdoc/>
        public override ReverseCallResponseContext GetResponseContext(PartitionedFilterResponse message)
            => message.CallContext;
    }
}
