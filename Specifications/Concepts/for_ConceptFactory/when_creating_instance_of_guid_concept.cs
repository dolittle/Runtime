// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    [Subject(typeof(ConceptFactory))]
    public class when_creating_instance_of_guid_concept
    {
        const string guid_value_as_string = "4AB92720-3138-4A7B-A7E9-2A49F6624736";
        static Guid guid;
        static GuidConcept result;

        Establish context = () => guid = Guid.Parse(guid_value_as_string);

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(GuidConcept), guid) as GuidConcept;

        It should_hold_the_correct_guid_value = () => result.Value.ToString().ToUpper().ShouldEqual(guid_value_as_string);
    }
}
