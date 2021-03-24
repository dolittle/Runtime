// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Projections.Store
{
    /// <summary>
    /// Represents the definition of a projection.
    /// </summary>
    public record Projection(
        ProjectionDefinition Definition,
        IEnumerable<string> ReadModels);
}
