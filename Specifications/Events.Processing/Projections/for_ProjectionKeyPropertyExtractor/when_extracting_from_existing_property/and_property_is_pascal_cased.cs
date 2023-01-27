// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Dolittle.Runtime.Projections.Store;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor.when_extracting_from_existing_property;

public class and_property_is_pascal_cased : given.all_dependencies
{
    static bool result;
    static ProjectionKey key;
    static content_structure json_object;

    Establish context = () =>
    {
        json_object = content_structure.create();
    };

    Because of = () => result = extractor.TryExtract(JsonSerializer.Serialize(json_object), nameof(json_object.CaseSensitive), out key);

    It should_return_true = () => result.Should().BeTrue();
    It should_set_key_correct_value = () => key.Value.Should().Be("CaseSensitive");
}