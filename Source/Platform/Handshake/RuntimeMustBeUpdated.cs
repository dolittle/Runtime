// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// The <see cref="Failure"/> that is returned when the Contracts version of the Client SDK is too old to be used with the current Runtime.
/// </summary>
public record RuntimeMustBeUpdated(Version MinimumContractsVersion) : Failure(
    HandshakeFailures.RuntimeMustBeUpdated,
    $"This version of the Runtime is too old to be used with the SDK you are currently using. Please upgrade your Runtime to a version that supports Contracts {MinimumContractsVersion}.");
