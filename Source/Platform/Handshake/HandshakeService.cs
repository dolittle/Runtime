// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Handshake.Contracts.Handshake;
using Environment = Dolittle.Runtime.Execution.Environment;
using Version = Dolittle.Runtime.Versioning.Version;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the implementation of.
/// </summary>
public class HandshakeService : HandshakeBase
{
    readonly IResolvePlatformEnvironment _platformEnvironment;
    readonly IVerifyContractsCompatibility _contractsCompatibility;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandshakeService"/> class.
    /// </summary>
    /// <param name="platformEnvironment">The <see cref="IResolvePlatformEnvironment"/>.</param>
    /// <param name="contractsCompatibility"></param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public HandshakeService(IResolvePlatformEnvironment platformEnvironment, IVerifyContractsCompatibility contractsCompatibility, ILogger logger)
    {
        _platformEnvironment = platformEnvironment;
        _contractsCompatibility = contractsCompatibility;
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task<HandshakeResponse> Handshake(HandshakeRequest request, ServerCallContext context)
    {
        try
        {
            var runtimeVersion = VersionInfo.CurrentVersion;
            var runtimeContractsVersion = Contracts.VersionInfo.CurrentVersion.ToVersion();
            var sdk = request.Sdk;
            var sdkVersion = request.SdkVersion.ToVersion();
            var headVersion = request.HeadVersion.ToVersion();
            var headContractsVersion = request.ContractsVersion.ToVersion();
            Log.HeadInitiatedHandshake(_logger, sdk, sdkVersion, headVersion, headContractsVersion);
            if (VersionsAreIncompatible(runtimeVersion, runtimeContractsVersion, headContractsVersion, out var failedResponse))
            {
                Log.HeadAndRuntimeContractsIncompatible(_logger, headContractsVersion, runtimeContractsVersion);
                return failedResponse;
            }
            var (microserviceId, environment) = await _platformEnvironment.Resolve().ConfigureAwait(false);
            Log.SuccessfulHandshake(_logger, runtimeVersion, microserviceId, runtimeContractsVersion, environment, headContractsVersion);
            return CreateSuccessfulResponse(microserviceId, environment, runtimeVersion, runtimeContractsVersion);
        }
        catch (Exception ex)
        {
            Log.ErrorWhilePerformingHandshake(_logger, ex);
            return CreateFailedResponse(ex.Message);
        }
    }

    static HandshakeResponse CreateSuccessfulResponse(MicroserviceId microserviceId, Environment environment, Version runtimeVersion, Version runtimeContractsVersion)
        => new()
        {
            Environment = environment,
            MicroserviceId = microserviceId.ToProtobuf(),
            RuntimeVersion = runtimeVersion.ToProtobuf(),
            ContractsVersion = runtimeContractsVersion.ToProtobuf()
        };

    static HandshakeResponse CreateFailedResponse(FailureReason reason)
        => new()
        {
            Failure = new Failure
            {
                Id = FailureId.Other.ToProtobuf(),
                Reason = reason
            }
        };

    bool VersionsAreIncompatible(Version runtimeVersion, Version runtimeContractsVersion, Version headContractsVersion, out HandshakeResponse failedResponse)
    {
        failedResponse = null;
        if (_contractsCompatibility.IsCompatible(runtimeContractsVersion, headContractsVersion))
        {
            return false;
        }
        
        failedResponse = CreateFailedResponse($"Runtime version {runtimeVersion} uses contracts version {runtimeContractsVersion} which is not compatible with the Head's version {headContractsVersion} of contracts");
        return true;
    }
}
