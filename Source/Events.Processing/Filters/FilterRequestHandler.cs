// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a system that can handle filter requests.
    /// </summary>
    /// <typeparam name="TRequest">The filter request <see cref="IMessage" />.</typeparam>
    /// <typeparam name="TResponse">The filter response <see cref="IMessage" />.</typeparam>
    public class FilterRequestHandler<TRequest, TResponse>
        where TRequest : IMessage
        where TResponse : IMessage
    {
        readonly IReverseCallDispatcher<TResponse, TRequest> _callDispatcher;
        readonly Func<TResponse, IFilterResult> _responseToResultCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRequestHandler{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="callDispatcher">The reverse call dispatcher.</param>
        /// <param name="responseToResultCallback">The callback for converting response to <see cref="IFilterResult" />.</param>
        public FilterRequestHandler(IReverseCallDispatcher<TResponse, TRequest> callDispatcher, Func<TResponse, IFilterResult> responseToResultCallback)
        {
            _callDispatcher = callDispatcher;
            _responseToResultCallback = responseToResultCallback;
        }

        /// <summary>
        /// Processes the <see cref="ProcessingRequestProxy{T}" /> for generic parameter TRequest.
        /// </summary>
        /// <param name="filterResult">The <see cref="ProcessingRequestProxy{T}" />.</param>
        /// <returns>The remote processing task that returns a <see cref="IFilterResult" />.</returns>
        public async Task<IFilterResult> Process(ProcessingRequestProxy<TRequest> filterResult)
        {
            IFilterResult result = null;
            await _callDispatcher.Call(filterResult, response => result = _responseToResultCallback(response)).ConfigureAwait(false);
            return result;
        }
    }
}