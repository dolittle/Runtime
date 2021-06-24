// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_CompareFilterDefinitions.when_checking_if_definitions_are_equal
{
    public class and_definitions_does_not_have_the_same_public_value : given.all_dependencies
    {
        static IFilterDefinition persisted;
        static IFilterDefinition registered;

        Establish context = () =>
        {
            persisted = new FilterDefinition(source_stream, target_stream, true);
            registered = new PublicFilterDefinition(source_stream, target_stream);
        };

        static FilterValidationResult result;
        Because of = () => result = definition_comparer.DefinitionsAreEqual(persisted, registered);

        It should_fail_validation = () => result.Success.ShouldBeFalse();
    }
}
