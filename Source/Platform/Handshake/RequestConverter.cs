// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Dolittle.Runtime.Client;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents an implementation of <see cref="IRequestConverter"/>.
/// </summary>
public class RequestConverter : IRequestConverter
{
    /// <inheritdoc />
    public bool TryConvert(HandshakeRequest request, [NotNullWhen(true)] out Request? parsed, [NotNullWhen(false)] out Failure? failure)
    {
        parsed = null;
        if (string.IsNullOrWhiteSpace(request.SdkIdentifier))
        {
            failure = new MissingHandshakeInformation(nameof(request.SdkIdentifier));
            return false;
        }
        if (request.HeadVersion == null)
        {
            failure = new MissingHandshakeInformation(nameof(request.HeadVersion));
            return false;
        }
        if (request.SdkVersion == null)
        {
            failure = new MissingHandshakeInformation(nameof(request.SdkVersion));
            return false;
        }
        if (request.ContractsVersion == null)
        {
            failure = new MissingHandshakeInformation(nameof(request.ContractsVersion));
            return false;
        }
        if (request.TimeSpent == null)
        {
            failure = new MissingHandshakeInformation(nameof(request.TimeSpent));
            return false;
        }
        failure = null;
        parsed = new Request(
            request.SdkIdentifier,
            request.SdkVersion.ToVersion(),
            request.HeadVersion.ToVersion(),
            request.ContractsVersion.ToVersion(),
            request.Attempt,
            request.TimeSpent.ToTimeSpan(),
            request.BuildResults?.ToBuildResults() ?? BuildResults.Empty);
        return true;
    }
}
