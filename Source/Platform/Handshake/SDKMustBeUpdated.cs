// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// The <see cref="Failure"/> that is returned when the Contracts version of the Client SDK is too old to be used with the current Runtime.
/// </summary>
public record SDKMustBeUpdated(Version MinimumContractsVersion) : Failure(
    HandshakeFailures.SDKMustBeUpdated,
    $"The version of your currently used SDK is too old to be used with this version of the Runtime. Please upgrade your Runtime to a version that supports Contracts {MinimumContractsVersion}.");
