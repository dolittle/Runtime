// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;

/// <summary>
/// Exception that gets thrown when trying to convert an unsupported type of <see cref="IFilterDefinition"/> in <see cref="Extensions"/>.
/// </summary>
public class UnsupportedFilterDefinitionType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedFilterDefinitionType"/> class.
    /// </summary>
    /// <param name="filter">The unsupported filter.</param>
    public UnsupportedFilterDefinitionType(IFilterDefinition filter)
        : base($"IFilterDefinition implementation {filter.GetType()} is not supported.")
    {
    }
}