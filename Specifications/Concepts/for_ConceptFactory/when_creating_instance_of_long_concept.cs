// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    public class when_creating_instance_of_long_concept
    {
        const long long_value = 42;

        static LongConcept result;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(LongConcept), long_value) as LongConcept;

        It should_hold_the_correct_long_value = () => result.Value.ShouldEqual(long_value);
    }
}
