// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_Extensions
{
    public class when_converting_concept_of_guid_back_and_forth
    {
        class GuidConcept : ConceptAs<Guid>
        {
        }

        static GuidConcept concept;
        static GuidConcept result;

        Establish context = () => concept = new GuidConcept { Value = Guid.NewGuid() };

        Because of = () => result = concept.ToProtobuf().To<GuidConcept>();

        It should_be_exactly_the_same = () => result.ShouldEqual(concept);
    }
}