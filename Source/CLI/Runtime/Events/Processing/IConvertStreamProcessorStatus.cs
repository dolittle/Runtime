// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using ManagementContracts = Dolittle.Runtime.Events.Processing.Management.Contracts;

namespace Dolittle.Runtime.CLI.Runtime.Events.Processing;

/// <summary>
/// Defines a system that can convert Stream Processor statuses from Contracts to Runtime representation.
/// </summary>
public interface IConvertStreamProcessorStatus
{
    /// <summary>
    /// Converts a set of <see cref="ManagementContracts.TenantScopedStreamProcessorStatus"/> to <see cref="TenantScopedStreamProcessorStatus"/>.
    /// </summary>
    /// <param name="statuses">The statuses to convert.</param>
    /// <returns>The converted statuses.</returns>
    IEnumerable<TenantScopedStreamProcessorStatus> Convert(IEnumerable<ManagementContracts.TenantScopedStreamProcessorStatus> statuses);
}
