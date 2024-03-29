// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Client;
using Dolittle.Runtime.Diagnostics.OpenTelemetry;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Dolittle.Runtime.Handshake.Contracts.Handshake;
using Version = Dolittle.Runtime.Domain.Platform.Version;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the implementation of.
/// </summary>
[PrivateService]
public class HandshakeService : HandshakeBase
{
    readonly IResolvePlatformEnvironment _platformEnvironment;
    readonly IRequestConverter _requestConverter;
    readonly IVerifyContractsCompatibility _contractsCompatibility;
    readonly IBuildResultsForHeads _buildResultsForHeads;
    readonly IOptions<OpenTelemetryConfiguration> _openTelemetryConfig;
    readonly ILogger _logger;
    
    readonly Version _runtimeVersion;
    readonly Version _runtimeContractsVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandshakeService"/> class.
    /// </summary>
    /// <param name="platformEnvironment">The <see cref="IResolvePlatformEnvironment"/> to use for resolving the current Runtime environment.</param>
    /// <param name="requestConverter">The <see cref="IRequestConverter"/> to use to parse incoming requests.</param>
    /// <param name="contractsCompatibility">The <see cref="IVerifyContractsCompatibility"/> to use to compare Contracts versions.</param>
    /// <param name="buildResultsForHeads">The <see cref="IBuildResultsForHeads"/>.</param>
    /// <param name="openTelemetryConfig">The <see cref="openTelemetryConfig"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    public HandshakeService(
        IResolvePlatformEnvironment platformEnvironment,
        IRequestConverter requestConverter,
        IVerifyContractsCompatibility contractsCompatibility,
        IBuildResultsForHeads buildResultsForHeads,
        IOptions<OpenTelemetryConfiguration> openTelemetryConfig,
        ILogger logger)
    {
        _platformEnvironment = platformEnvironment;
        _requestConverter = requestConverter;
        _contractsCompatibility = contractsCompatibility;
        _buildResultsForHeads = buildResultsForHeads;
        _openTelemetryConfig = openTelemetryConfig;
        _logger = logger;

        _runtimeVersion = VersionInfo.CurrentVersion;
        _runtimeContractsVersion = Contracts.VersionInfo.CurrentVersion.ToVersion();
    }

    /// <inheritdoc />
    public override async Task<HandshakeResponse> Handshake(HandshakeRequest request, ServerCallContext context)
    {
        try
        {
            if (!_requestConverter.TryConvert(request, out var parsedRequest, out var failure))
            {
                Log.RequestParsingFailed(_logger, failure.Reason);
                return FailedResponse(failure);
            }
            
            Log.HeadInitiatedHandshake(
                _logger,
                parsedRequest.Attempt,
                parsedRequest.HeadVersion,
                parsedRequest.SDK,
                parsedRequest.SDKVersion,
                parsedRequest.ContractsVersion);

            if (parsedRequest.BuildResults is not null)
            {
                _buildResultsForHeads.AddFor(Guid.Empty, parsedRequest.BuildResults);
            }

            switch (_contractsCompatibility.CheckCompatibility(_runtimeContractsVersion, parsedRequest.ContractsVersion))
            {
                case ContractsCompatibility.ClientTooOld:
                    Log.ClientContractsVersionTooOld(_logger, parsedRequest.ContractsVersion, _runtimeContractsVersion);
                    return FailedResponse(new SDKMustBeUpdated(_runtimeContractsVersion));
                case ContractsCompatibility.RuntimeTooOld:
                    Log.RuntimeContractsVersionTooOld(_logger, parsedRequest.ContractsVersion, _runtimeContractsVersion);
                    return FailedResponse(new RuntimeMustBeUpdated(parsedRequest.ContractsVersion));
            }
            
            var platformEnvironment = await _platformEnvironment.Resolve().ConfigureAwait(false);
            Log.SuccessfulHandshake(
                _logger,
                parsedRequest.HeadVersion,
                parsedRequest.SDK,
                parsedRequest.SDKVersion,
                platformEnvironment.MicroserviceId,
                platformEnvironment.Environment,
                parsedRequest.Attempt,
                parsedRequest.TimeSpent);

            var otlpConfig = _openTelemetryConfig.Value;
            var response = new HandshakeResponse
            {
                RuntimeVersion = _runtimeVersion.ToProtobuf(),
                ContractsVersion = _runtimeContractsVersion.ToProtobuf(),
                CustomerId = platformEnvironment.CustomerId.ToProtobuf(),
                CustomerName = platformEnvironment.CustomerName.Value,
                ApplicationId = platformEnvironment.ApplicationId.ToProtobuf(),
                ApplicationName = platformEnvironment.ApplicationName.Value,
                MicroserviceId = platformEnvironment.MicroserviceId.ToProtobuf(),
                MicroserviceName = platformEnvironment.MicroserviceName.Value,
                EnvironmentName = platformEnvironment.Environment.Value,
            };

            if (otlpConfig.Tracing && !string.IsNullOrEmpty(otlpConfig.Endpoint))
            {
                response.OtlpEndpoint = otlpConfig.Endpoint;
            }

            return response;
        }
        catch (Exception ex)
        {
            Log.ErrorWhilePerformingHandshake(_logger, ex);
            return FailedResponse(new Failure{Id = FailureId.Other.ToProtobuf(), Reason = ex.Message});
        }
    }

    static HandshakeResponse FailedResponse(Failure failure)
        => new() {Failure = failure};
}
