// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Defines a converter that can convert Projection definitions between Contracts and Runtime representations.
/// </summary>
public interface IConvertProjectionDefinitions
{
    /// <summary>
    /// Converts event selectors from Contracts to Runtime representation.
    /// </summary>
    /// <param name="selectors">The <see cref="Contracts.ProjectionEventSelector">selectors</see> to convert.</param>
    /// <returns>The converted <see cref="ProjectionEventSelector">selectors</see>.</returns>
    IEnumerable<ProjectionEventSelector> ToRuntimeEventSelectors(IEnumerable<Contracts.ProjectionEventSelector> selectors);
    
    /// <summary>
    /// Converts event selectors from Runtime to Contracts representation.
    /// </summary>
    /// <param name="selectors">The <see cref="ProjectionEventSelector">selectors</see> to convert.</param>
    /// <returns>The converted <see cref="Contracts.ProjectionEventSelector">selectors</see>.</returns>
    IEnumerable<Contracts.ProjectionEventSelector> ToContractsEventSelectors(IEnumerable<ProjectionEventSelector> selectors);

    /// <summary>
    /// Converts a copy specification from Contracts to Runtime representation.
    /// </summary>
    /// <param name="copies">The <see cref="Contracts.ProjectionCopies">copies</see> to convert.</param>
    /// <returns>The converted <see cref="ProjectionCopySpecification">copies</see>.</returns>
    ProjectionCopySpecification ToRuntimeCopySpecification(Contracts.ProjectionCopies copies);

    /// <summary>
    /// Converts a copy specification from Runtime to Contracts representation.
    /// </summary>
    /// <param name="copies">The <see cref="ProjectionCopySpecification">copies</see> to convert.</param>
    /// <returns>The converted <see cref="Contracts.ProjectionCopies">copies</see>.</returns>
    Contracts.ProjectionCopies ToContractsCopySpecification(ProjectionCopySpecification copies);
}
