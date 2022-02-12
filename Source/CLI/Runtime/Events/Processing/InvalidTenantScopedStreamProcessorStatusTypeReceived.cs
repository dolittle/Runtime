// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using ManagementContracts = Dolittle.Runtime.Events.Processing.Management.Contracts;

namespace Dolittle.Runtime.CLI.Runtime.Events.Processing;

/// <summary>
/// Exception that gets thrown when the Runtime returns a <see cref="TenantScopedStreamProcessorStatus"/> type that is invalid.
/// </summary>
public class InvalidTenantScopedStreamProcessorStatusTypeReceived : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidTenantScopedStreamProcessorStatusTypeReceived"/> class.
    /// </summary>
    /// <param name="statusType">The status type that was received.</param>
    public InvalidTenantScopedStreamProcessorStatusTypeReceived(ManagementContracts.TenantScopedStreamProcessorStatus.StatusOneofCase statusType)
        : base($"Invalid {nameof(TenantScopedStreamProcessorStatus)} received: ${statusType}")
    {
    }
}
