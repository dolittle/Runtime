
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_converting_concept_of_guid_to_and_from_protobuf
    {
        class guid_concept : ConceptAs<Guid>
        {

        }
        static guid_concept concept;
        static System.Protobuf.guid protobuf;
        static guid_concept result;

        Establish context = () => concept = new guid_concept { Value = Guid.NewGuid() };

        Because of = () => 
        {
            protobuf = concept.ToProtobuf();
            result = protobuf.ToConcept<guid_concept>();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(concept);
    }
}