// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Platform.Handshake;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Error, "Failed to parse handshake request because {Reason}")]
    internal static partial void RequestParsingFailed(ILogger logger, FailureReason reason);

    [LoggerMessage(0, LogLevel.Debug, "Handshake attempt {Attempt} initiated by a Head version {HeadVersion} using the {SDKIdentifier} SDK {SDKVersion} with Contracts {HeadContractsVersion}")]
    internal static partial void HeadInitiatedHandshake(ILogger logger, HandshakeAttempt attempt, Version headVersion, SDKIdentifier sdkIdentifier, Version sdkVersion, Version headContractsVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "The Client that initiated the handshake is using a Contracts version {HeadContractsVersion} that is older than the Contracts version {RuntimeContractsVersion} of this Runtime")]
    internal static partial void ClientContractsVersionTooOld(ILogger logger, Version headContractsVersion, Version runtimeContractsVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "The Client that initiated the handshake is using a Contracts version {HeadContractsVersion} that is newer than the Contracts version {RuntimeContractsVersion} of this Runtime")]
    internal static partial void RuntimeContractsVersionTooOld(ILogger logger, Version headContractsVersion, Version runtimeContractsVersion);

    [LoggerMessage(0, LogLevel.Information, "Handshake successful with Head version {HeadVersion} using {SDKIdentifier} SDK version {SDKVersion} for microservice {Microservice} in environment {Environment} at attempt {Attempt} after {TimeSpent}")]
    internal static partial void SuccessfulHandshake(ILogger logger, Version headVersion, SDKIdentifier sdkIdentifier, Version sdkVersion, MicroserviceId microservice, Environment environment, HandshakeAttempt attempt, HandshakeTimeSpent timeSpent);
    
    [LoggerMessage(0, LogLevel.Error, "An error occurred while performing handshake")]
    internal static partial void ErrorWhilePerformingHandshake(ILogger logger, Exception ex);
}
