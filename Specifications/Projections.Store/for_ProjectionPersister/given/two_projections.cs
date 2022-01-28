// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.given;

public class two_projections
{
    protected static ProjectionDefinition projection_one;
    protected static ProjectionId projection_one_id;
    protected static ScopeId projection_one_scope;
    
    protected static ProjectionDefinition projection_two;
    protected static ProjectionId projection_two_id;
    protected static ScopeId projection_two_scope;

    protected static ProjectionKey key_one;
    protected static ProjectionKey key_two;

    protected static ProjectionState state_one;
    protected static ProjectionState state_two;

    Establish context = () =>
    {
        projection_one_id = Guid.Parse("1198a4ae-de3d-4e38-9c8d-06f881852801");
        projection_one_scope = Guid.Parse("e45adb3f-f09c-4ab7-988d-dbbd8550df49");
        projection_one = new ProjectionDefinition(
            projection_one_id,
            projection_one_scope,
            Enumerable.Empty<ProjectionEventSelector>(), 
            "{}",
            new ProjectionCopySpecification(
                CopyToMongoDBSpecification.Default
            )
        );
        
        projection_two_id = Guid.Parse("bf279ef0-a895-4f98-943d-cb0186695467");
        projection_two_scope = Guid.Parse("4c9f9134-95fb-4d31-b027-7127995370f1");
        projection_two = new ProjectionDefinition(
            projection_two_id,
            projection_two_scope,
            Enumerable.Empty<ProjectionEventSelector>(), 
            "{}",
            new ProjectionCopySpecification(
                CopyToMongoDBSpecification.Default
            )
        );

        key_one = "key one";
        key_two = "key two";

        state_one = "{ \"state\": 1 }";
        state_two = "{ \"state\": 2 }";
    };
}