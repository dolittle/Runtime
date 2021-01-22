// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    [Subject(typeof(ConceptFactory))]
    public class when_creating_instance_of_datetime_concept_with_value_as_datetime
    {
        static DateTimeConcept result;
        static DateTime now;

        Establish context = () => now = DateTime.Now;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(DateTimeConcept), now) as DateTimeConcept;

        It should_be_the_value_of_the_datetime = () => result.Value.ShouldEqual(now);
    }
}