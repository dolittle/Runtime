// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Store.for_ProjectionKeyToEventSourceIdConverter.when_getting_event_source_id_for
{
    public class a_special_value : given.a_converter
    {
        static ProjectionKey projection_key;

        Establish context = () => projection_key = "this_is the.special key-used for:specifications";

        static EventSourceId event_source_id;

        Because of = () => event_source_id = converter.GetEventSourceIdFor(projection_key);

        It should_return_a_special_event_source = () => event_source_id.Value.ShouldEqual(Guid.Parse("f5fffaff-66b4-f316-4b4d-54a435b50d2a"));
    }
}
