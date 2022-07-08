// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

public record ProjectionStatus(
    ProjectionId Id,
    ScopeId Scope,
    JObject InitialState,
    IEnumerable<ProjectionEventSelector> Events,
    ProjectionCopySpecification Copies,
    ProjectionAlias Alias,
    IEnumerable<UnpartitionedTenantScopedStreamProcessorStatus> States)
{
    
    /// <summary>
    /// Gets a value indicating whether the Projection has an alias.
    /// </summary>
    public bool HasAlias => !Alias.Equals(ProjectionAlias.NotSet);
        
    /// <summary>
    /// Gets a value indicating whether the Projection is in the default Scope.
    /// </summary>
    public bool IsInDefaultScope => Scope.Equals(ScopeId.Default);
}
