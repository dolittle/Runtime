// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents an implementation of <see cref="IVerifyContractsCompatibility"/>.
/// </summary>
public class VerifyContractsCompatibility : IVerifyContractsCompatibility
{
    /// <inheritdoc />
    public ContractsCompatibility CheckCompatibility(Version runtimeContractsVersion, Version headContractsVersion)
    {
        if (headContractsVersion.Major > runtimeContractsVersion.Major)
        {
            return ContractsCompatibility.RuntimeTooOld;
        }
        
        if (headContractsVersion.Major < runtimeContractsVersion.Major)
        {
            return ContractsCompatibility.ClientTooOld;
        }
        
        if (headContractsVersion.Minor > runtimeContractsVersion.Minor)
        {
            return ContractsCompatibility.RuntimeTooOld;
        }

        return ContractsCompatibility.Compatible;
    }
}
