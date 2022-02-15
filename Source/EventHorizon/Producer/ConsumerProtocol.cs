// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents the <see cref="IConsumerProtocol" />.
/// </summary>
public class ConsumerProtocol : IConsumerProtocol
{
    /// <inheritdoc/>
    public ConsumerSubscriptionArguments ConvertConnectArguments(ConsumerSubscriptionRequest arguments)
    {
        var executionContext = arguments.CallContext.ExecutionContext.ToExecutionContext();
        return new ConsumerSubscriptionArguments(
            executionContext,
            executionContext.Microservice,
            executionContext.Tenant,
            arguments.TenantId.ToGuid(),
            arguments.StreamId.ToGuid(),
            arguments.PartitionId,
            arguments.StreamPosition);
    }

    /// <inheritdoc/>
    public SubscriptionResponse CreateFailedConnectResponse(FailureReason failureMessage)
        => new() { Failure = new Dolittle.Protobuf.Contracts.Failure { Id = SubscriptionFailures.MissingSubscriptionArguments.Value.ToProtobuf(), Reason = failureMessage } };

    /// <inheritdoc/>
    public ReverseCallArgumentsContext GetArgumentsContext(ConsumerSubscriptionRequest message)
        => message.CallContext;

    /// <inheritdoc/>
    public ConsumerSubscriptionRequest GetConnectArguments(EventHorizonConsumerToProducerMessage message)
        => message.SubscriptionRequest;

    /// <inheritdoc/>
    public Pong GetPong(EventHorizonConsumerToProducerMessage message)
        => message.Pong;

    /// <inheritdoc/>
    public ConsumerResponse GetResponse(EventHorizonConsumerToProducerMessage message)
        => message.Response;

    /// <inheritdoc/>
    public ReverseCallResponseContext GetResponseContext(ConsumerResponse message)
        => message.CallContext;

    /// <inheritdoc/>
    public void SetConnectResponse(SubscriptionResponse arguments, EventHorizonProducerToConsumerMessage message)
        => message.SubscriptionResponse = arguments;

    /// <inheritdoc/>
    public void SetPing(EventHorizonProducerToConsumerMessage message, Ping ping)
        => message.Ping = ping;

    /// <inheritdoc/>
    public void SetRequest(ConsumerRequest request, EventHorizonProducerToConsumerMessage message)
        => message.Request = request;

    /// <inheritdoc/>
    public void SetRequestContext(ReverseCallRequestContext context, ConsumerRequest request)
        => request.CallContext = context;

    /// <inheritdoc/>
    public ConnectArgumentsValidationResult ValidateConnectArguments(ConsumerSubscriptionArguments arguments)
        => ConnectArgumentsValidationResult.Ok;
}
