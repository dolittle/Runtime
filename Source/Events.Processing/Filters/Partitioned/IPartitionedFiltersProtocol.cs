// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Runtime.Events.Processing.Filters.Partitioned;

/// <summary>
/// Defines the protocol for partitioned filters.
/// </summary>
public interface IPartitionedFiltersProtocol : IFiltersProtocol<PartitionedFilterClientToRuntimeMessage, PartitionedFilterRegistrationRequest, PartitionedFilterResponse, PartitionedFilterRegistrationArguments>
{
}