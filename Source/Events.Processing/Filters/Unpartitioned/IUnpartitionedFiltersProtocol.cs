// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Runtime.Events.Processing.Filters.Unpartitioned
{
    /// <summary>
    /// Defines the protocol for unpartitioned filters.
    /// </summary>
    public interface IUnpartitionedFiltersProtocol : IFiltersProtocol<FilterClientToRuntimeMessage, FilterRegistrationRequest, FilterResponse, UnpartitionedFilterRegistrationArguments>
    {
    }
}
