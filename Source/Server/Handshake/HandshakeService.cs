// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Handshake.Contracts.Handshake;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Runtime.Server.Handshake;

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
            var headContractsVersion = request.ContractsVersion.ToVersion();
            if (_contractsCompatibility.IsCompatible(runtimeContractsVersion, request.ContractsVersion.ToVersion()))
            {
                var (microservice, environment) = await _platformEnvironment.Resolve().ConfigureAwait(false);
                return new HandshakeResponse
                {
                    Environment = environment,
                    MicroserviceId = microservice.ToProtobuf(),
                    ContractsVersion = Contracts.VersionInfo.CurrentVersion,
                    RuntimeVersion = VersionInfo.CurrentVersion.ToProtobuf()
                };
            }
            Log.HeadAndRuntimeContractsIncompatible(_logger, headContractsVersion, runtimeContractsVersion);
            return CreateFailedResponse($"Runtime version {runtimeVersion} uses contracts version {runtimeContractsVersion} which is not compatible with the Head's version {headContractsVersion} of contracts");
        }
        catch (Exception ex)
        {
            Log.ErrorWhilePerformingHandshake(_logger, ex);
            return CreateFailedResponse(ex.Message);
        }
    }

    static HandshakeResponse CreateFailedResponse(FailureReason reason)
        => new()
        {
            Failure = new Failure
            {
                Id = FailureId.Other.ToProtobuf(),
                Reason = reason
            }
        };
}
