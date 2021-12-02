// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_CompareFilterDefinitions.given;

public class all_dependencies
{
    protected static StreamId source_stream;
    protected static StreamId target_stream;
    protected static CompareFilterDefinitions definition_comparer;

    Establish context = () =>
    {
        source_stream = Guid.Parse("e3b6f228-909e-4d47-8aae-45992b2053b7");
        target_stream = Guid.Parse("05dd5fce-427c-4d3f-ba41-6a706ba42a45");

        definition_comparer = new CompareFilterDefinitions();
    };
}