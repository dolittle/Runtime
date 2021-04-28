// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating
{
    public class and_the_filter_definition_type_has_changed : given.all_dependencies
    {
        Establish context = () =>
        {
            filter_definitions
                .Setup(_ => _.TryGetFromStream(scope_id, filter_target_stream, cancellation_token))
                .Returns(Task.FromResult(new Try<IFilterDefinition>(true, different_filter_definition)));

            var validationResult = new FilterValidationResult();
            definition_comparer
                .Setup(_ => _.DefinitionsAreEqual(different_filter_definition, filter_definition, out validationResult))
                .Returns(true);
        };

        static FilterValidationResult result;
        Because of = () => result = filter_validators().Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
    }
}
