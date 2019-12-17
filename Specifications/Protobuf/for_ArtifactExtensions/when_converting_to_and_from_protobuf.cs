// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Relativity.Microservice;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_ArtifactExtensions
{
    public class when_converting_to_and_from_protobuf
    {
        static Artifacts.Artifact original;
        static Artifact protobuf;
        static Artifacts.Artifact result;

        Establish context = () => original = new Artifacts.Artifact(Dolittle.Artifacts.ArtifactId.New(), Artifacts.ArtifactGeneration.First);

        Because of = () =>
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToArtifact();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}