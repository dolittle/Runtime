// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor.when_extracting_from_existing_property
{
    public class and_inner_property_does_not_exist : given.all_dependencies
    {
        static bool result;
        static ProjectionKey key;
        static content_structure json_object;

        Establish context = () =>
        {
            json_object = content_structure.create();
        };

        Because of = () => result = extractor.TryExtract(
            serializer.ToJson(json_object),
            $"{nameof(json_object.inner_structure)}.not_exist",
            out key);

        It should_return_false = () => result.ShouldBeFalse();
        It should_set_key_to_null = () => key.ShouldBeNull();
    }
}