// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Dolittle.Runtime.Rudimentary;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IInitiateReverseCallServices" />.
    /// </summary>
    public class InitiateReverseCallServices : IInitiateReverseCallServices
    {
        static readonly string ArgumentsNotReceived = "Connection arguments were not received";
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitiateReverseCallServices" >/ class.
        /// </summary>
        /// <param name="reverseCallDispatchers"></param>
        /// <param name="logger"></param>
        public InitiateReverseCallServices(IReverseCallDispatchers reverseCallDispatchers, ILogger logger)
        {
            _reverseCallDispatchers = reverseCallDispatchers;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Try<(IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> dispatcher, TRuntimeConnectArguments arguments)>> Connect<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse, TRuntimeConnectArguments>(
                IAsyncStreamReader<TClientMessage> runtimeStream,
                IServerStreamWriter<TServerMessage> clientStream,
                ServerCallContext context,
                IReverseCallServiceProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse, TRuntimeConnectArguments> protocol,
                CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class
            where TRuntimeConnectArguments : class
        {
            var dispatcher = _reverseCallDispatchers.GetFor(runtimeStream, clientStream, context, protocol);
            _logger.LogTrace("Waiting for connection arguments");
            if (!await dispatcher.ReceiveArguments(cancellationToken).ConfigureAwait(false))
            {
                _logger.LogWarning(ArgumentsNotReceived);
                await dispatcher.Reject(protocol.CreateFailedConnectResponse(ArgumentsNotReceived), cancellationToken).ConfigureAwait(false);
                return new ConnectArgumentsNotReceived();
            }
            _logger.LogTrace("Received connection arguments");

            var connectArguments = protocol.ConvertConnectArguments(dispatcher.Arguments);
            var validationResult = protocol.ValidateConnectArguments(connectArguments);
            if (!validationResult.Success)
            {
                _logger.LogTrace("Connection arguments were not valid");
                return new ConnectArgumentsValidationFailed(validationResult.FailureReason);
            }

            return (dispatcher, connectArguments);
        }
    }
}
