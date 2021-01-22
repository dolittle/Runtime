// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    public class when_creating_instance_of_concept_with_no_default_constructor
    {
        const long long_value = 42;

        static ConceptWithNoDefaultConstructor result;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(ConceptWithNoDefaultConstructor), long_value) as ConceptWithNoDefaultConstructor;

        It should_hold_the_correct_long_value = () => result.Value.ShouldEqual(long_value);
    }
}
