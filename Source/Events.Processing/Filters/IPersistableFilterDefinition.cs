// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a marker interface on top of <see cref="IFilterDefinition" /> that marks the <see cref="IFilterDefinition" /> as persitable.
    /// </summary>
    public interface IPersistableFilterDefinition : IFilterDefinition
    {
    }
}