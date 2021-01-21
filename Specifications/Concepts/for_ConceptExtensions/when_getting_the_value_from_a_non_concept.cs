// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Specs.Concepts.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptExtensions
{
    [Subject(typeof(ConceptExtensions))]
    public class when_getting_the_value_from_a_non_concept : concepts
    {
        static string primitive_value = "ten";
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => primitive_value.GetConceptValue());

        It should_throw_an_argument_exception = () => exception.ShouldBeOfExactType<TypeIsNotAConcept>();
    }
}