// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Dolittle.Runtime.Rudimentary;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents an implementation of <see cref="IInitiateReverseCallServices" />.
/// </summary>
public class InitiateReverseCallServices : IInitiateReverseCallServices
{
    const string ArgumentsNotReceivedResponse = "Connection arguments were not received";
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
        CancellationToken cancellationToken,
        bool notValidateExecutionContext = false)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
        where TRuntimeConnectArguments : class
    {
        var dispatcher = _reverseCallDispatchers.GetFor(runtimeStream, clientStream, context, protocol);
        try
        {
            Log.WaitingForConnectionArguments(_logger);
            if (!await dispatcher.ReceiveArguments(cancellationToken, notValidateExecutionContext).ConfigureAwait(false))
            {
                Log.ConnectionArgumentsNotReceived(_logger);
                await dispatcher.Reject(protocol.CreateFailedConnectResponse(ArgumentsNotReceivedResponse), cancellationToken).ConfigureAwait(false);
                dispatcher.Dispose();
                return new ConnectArgumentsNotReceived();
            }
            
            Log.ReceivedConnectionArguments(_logger);
            var connectArguments = protocol.ConvertConnectArguments(dispatcher.Arguments);
            var validationResult = protocol.ValidateConnectArguments(connectArguments);
            if (validationResult.Success)
            {
                return (dispatcher, connectArguments);
            }

            Log.ReceivedInvalidConnectionArguments(_logger);
            await dispatcher.Reject(protocol.CreateFailedConnectResponse(validationResult.FailureReason), cancellationToken).ConfigureAwait(false);
            dispatcher.Dispose();
            return new ConnectArgumentsValidationFailed(validationResult.FailureReason);
        }
        catch
        {
            dispatcher.Dispose();
            throw;
        }
    }
}
