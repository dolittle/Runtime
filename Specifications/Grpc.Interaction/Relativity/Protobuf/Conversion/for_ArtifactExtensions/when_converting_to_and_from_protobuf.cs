/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_ArtifactExtensions
{
    public class when_converting_to_and_from_protobuf
    {
        static Dolittle.Artifacts.Artifact original;
        static Artifact protobuf;
        static Dolittle.Artifacts.Artifact result;

        Establish context = () => original = new Dolittle.Artifacts.Artifact(Dolittle.Artifacts.ArtifactId.New(), Dolittle.Artifacts.ArtifactGeneration.First);

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToArtifact();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}