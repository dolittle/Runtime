using System;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys.given
{
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

        public ProjectionDefinition Build() => new(Guid.NewGuid(), Guid.NewGuid(), selectors, "");
    }
}
