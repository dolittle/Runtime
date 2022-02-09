using System;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys.given;

public class projection_definition_builder
{
    public static projection_definition_builder create() => new();
    readonly List<ProjectionEventSelector> selectors = new();
    projection_definition_builder()
    {
    }

    public projection_definition_builder with_selector(ProjectionEventSelector selector)
    {
        selectors.Add(selector);
        return this;
    }

    public ProjectionDefinition Build() => new(
        Guid.Parse("4af56faf-6dca-4348-8790-c53972485d47"), 
        Guid.Parse("daa3841d-fbb4-455e-be49-9638194bbad8"),
        selectors, 
        "", 
        new ProjectionCopySpecification(CopyToMongoDBSpecification.Default));
}