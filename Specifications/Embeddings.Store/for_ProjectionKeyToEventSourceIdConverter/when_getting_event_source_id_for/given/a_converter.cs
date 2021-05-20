// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Store.for_ProjectionKeyToEventSourceIdConverter.when_getting_event_source_id_for.given
{
    public class a_converter
    {
        protected static ProjectionKeyToEventSourceIdConverter converter;

        Establish context = () =>
        {
            converter = new ProjectionKeyToEventSourceIdConverter();
        };
    }
}
