// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Dolittle.Runtime.Projections.Store;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor;

public class when_property_does_not_exist : given.all_dependencies
{
    static bool result;
    static ProjectionKey key;
    Because of = () => result = extractor.TryExtract(JsonSerializer.Serialize(content_structure.create()), "not_exist", out key);

    It should_fail = () => result.Should().BeFalse();
    It should_set_key_to_null = () => key.Should().BeNull();
}