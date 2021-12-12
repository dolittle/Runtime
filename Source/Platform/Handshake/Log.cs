// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Microsoft.Extensions.Logging;
using Environment = Dolittle.Runtime.Execution.Environment;
using Version = Dolittle.Runtime.Versioning.Version;

namespace Dolittle.Runtime.Platform.Handshake;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Handshake initiated by a Head v{HeadVersion} using the {SDK} SDK using version {HeadContractsVersion} Contracts")]
    internal static partial void HeadInitiatedHandshake(ILogger logger, string sdk, Version sdkVersion, Version headVersion, Version headContractsVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "Cannot perform handshake between Head and Runtime because the Head's version of contracts ({HeadContractsVersion}) is incompatible with the Runtime version of contracts ({RuntimeContractsVersion})")]
    internal static partial void HeadAndRuntimeContractsIncompatible(ILogger logger, Version headContractsVersion, Version runtimeContractsVersion);

    [LoggerMessage(0, LogLevel.Information, "Runtime v{RuntimeVersion} with Microservice ID {MicroserviceId} using version {RuntimeContractsVersion} Contracts running in environment {Environment} successfully performed handshake with Head using version {HeadContractsVersion} Contracts")]
    internal static partial void SuccessfulHandshake(ILogger logger, Version runtimeVersion, MicroserviceId microserviceId, Version runtimeContractsVersion, Environment environment, Version headContractsVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while performing handshake")]
    internal static partial void ErrorWhilePerformingHandshake(ILogger logger, Exception ex);
}
