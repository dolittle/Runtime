// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_CompareFilterDefinitions.when_checking_if_definitions_are_equal
{
    public class and_definitions_does_not_have_the_same_partitioned_value : given.all_dependencies
    {
        static IFilterDefinition persisted;
        static IFilterDefinition registered;

        Establish context = () =>
        {
            persisted = new FilterDefinition(source_stream, target_stream, true);
            registered = new FilterDefinition(source_stream, target_stream, false);
        };

        static bool result;
        static FilterValidationResult validationResult;
        Because of = () => result = definition_comparer.DefinitionsAreEqual(persisted, registered, out validationResult);

        It should_return_false = () => result.ShouldBeFalse();
        It should_return_a_failed_result = () => validationResult.Succeeded.ShouldBeFalse();
    }
}
