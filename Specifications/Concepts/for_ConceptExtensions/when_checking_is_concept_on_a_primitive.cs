// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptExtensions
{
    [Subject(typeof(ConceptExtensions))]
    public class when_checking_is_concept_on_a_primitive : Dolittle.Runtime.Specs.Concepts.given.concepts
    {
        static bool is_a_concept;

        Because of = () => is_a_concept = 1.GetType().IsConcept();

        It should_not_be_a_concept = () => is_a_concept.ShouldBeFalse();
    }
}