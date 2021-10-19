// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when the Runtime returns a <see cref="TenantScopedStreamProcessorStatus"/> type that is invalid.
    /// </summary>
    public class InvalidTenantScopedStreamProcessorStatusTypeReceived : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTenantScopedStreamProcessorStatusTypeReceived"/> class.
        /// </summary>
        /// <param name="statusType">The status type that was received.</param>
        public InvalidTenantScopedStreamProcessorStatusTypeReceived(Events.Processing.Management.Contracts.TenantScopedStreamProcessorStatus.StatusOneofCase statusType)
            : base($"Invalid {nameof(TenantScopedStreamProcessorStatus)} received: ${statusType}")
        {
        }
    }
}