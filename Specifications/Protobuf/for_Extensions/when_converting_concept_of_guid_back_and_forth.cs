// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_Extensions
{
    public class when_converting_concept_of_guid_back_and_forth
    {
        record GuidConcept(Guid value) : ConceptAs<Guid>(value)
        {
            public static implicit operator GuidConcept(Guid value) => new(value);
        }

        static GuidConcept concept;
        static GuidConcept result;

        Establish context = () => concept = new GuidConcept(Guid.NewGuid());

        Because of = () => result = concept.ToProtobuf().ToGuid();

        It should_be_exactly_the_same = () => result.ShouldEqual(concept);
    }
}