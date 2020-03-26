// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents a system that can handle event handler processing requests.
    /// </summary>
    /// <typeparam name="TRequest">The event handler processing request <see cref="IMessage" />.</typeparam>
    /// <typeparam name="TResponse">The event handler processing response <see cref="IMessage" />.</typeparam>
    public class EventHandlerProcessingRequestHandler<TRequest, TResponse>
        where TRequest : IMessage
        where TResponse : IMessage
    {
        readonly IReverseCallDispatcher<TResponse, TRequest> _callDispatcher;
        readonly Func<TResponse, IProcessingResult> _responseToResultCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProcessingRequestHandler{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="callDispatcher">The reverse call dispatcher.</param>
        /// <param name="responseToResultCallback">The callback for converting response to <see cref="IProcessingResult" />.</param>
        public EventHandlerProcessingRequestHandler(IReverseCallDispatcher<TResponse, TRequest> callDispatcher, Func<TResponse, IProcessingResult> responseToResultCallback)
        {
            _callDispatcher = callDispatcher;
            _responseToResultCallback = responseToResultCallback;
        }

        /// <summary>
        /// Processes the <see cref="ProcessingRequestProxy{T}" /> for generic parameter TRequest.
        /// </summary>
        /// <param name="processingRequest">The <see cref="ProcessingRequestProxy{T}" />.</param>
        /// <returns>The remote processing task that returns a <see cref="IProcessingResult" />.</returns>
        public async Task<IProcessingResult> Process(ProcessingRequestProxy<TRequest> processingRequest)
        {
            IProcessingResult result = null;
            await _callDispatcher.Call(processingRequest, response => result = _responseToResultCallback(response)).ConfigureAwait(false);
            return result;
        }
    }
}