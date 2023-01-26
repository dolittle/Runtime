// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon;

/// <summary>
/// Defines the protocol for public filters.
/// </summary>
public interface IPublicFiltersProtocol : IFiltersProtocol<PublicFilterClientToRuntimeMessage, PublicFilterRegistrationRequest, PartitionedFilterResponse, PublicFilterRegistrationArguments>
{
}