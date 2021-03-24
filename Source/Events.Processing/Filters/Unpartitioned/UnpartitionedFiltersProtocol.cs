// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Events.Processing.Filters.Unpartitioned
{
    /// <summary>
    /// Represents an implementation of <see cref="IUnpartitionedFiltersProtocol" />.
    /// </summary>
    public class UnpartitionedFiltersProtocol : FiltersProtocol<FilterClientToRuntimeMessage, FilterRegistrationRequest, FilterResponse, UnpartitionedFilterRegistrationArguments>, IUnpartitionedFiltersProtocol
    {
        /// <inheritdoc/>
        public override UnpartitionedFilterRegistrationArguments ConvertConnectArguments(FilterRegistrationRequest arguments)
            => new(arguments.CallContext.ExecutionContext.ToExecutionContext(), arguments.FilterId.ToGuid(), arguments.ScopeId.ToGuid());

        /// <inheritdoc/>
        public override ReverseCallArgumentsContext GetArgumentsContext(FilterRegistrationRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public override FilterRegistrationRequest GetConnectArguments(FilterClientToRuntimeMessage message)
            => message.RegistrationRequest;

        /// <inheritdoc/>
        public override Pong GetPong(FilterClientToRuntimeMessage message)
            => message.Pong;

        /// <inheritdoc/>
        public override FilterResponse GetResponse(FilterClientToRuntimeMessage message)
            => message.FilterResult;

        /// <inheritdoc/>
        public override ReverseCallResponseContext GetResponseContext(FilterResponse message)
            => message.CallContext;
    }
}
