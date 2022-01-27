// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.given;

public class a_projection
{
    protected static CollectionName collection_name;
    
    protected static ProjectionId projection_id;
    protected static ProjectionDefinition projection;

    protected static ProjectionKey projection_key;
    protected static ProjectionState projection_state;

    Establish context = () =>
    {
        collection_name = "collection_name";
        
        projection_id = "4396da1a-3490-4d6f-ad99-c4648ff9b526";
        
        projection = new ProjectionDefinition(
            projection_id,
            "f400e55b-5932-4cd0-b419-dd0e0c30236f",
            Enumerable.Empty<ProjectionEventSelector>(),
            "{}",
            new ProjectionCopySpecification(
                new CopyToMongoDBSpecification(
                    true,
                    collection_name,
                    ImmutableDictionary<ProjectionField, ConversionBSONType>.Empty
                )
            )
        );

        projection_key = "projection key";
        projection_state = "{}";
    };
}