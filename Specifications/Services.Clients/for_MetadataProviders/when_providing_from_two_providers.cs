// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Clients.for_MetadataProviders;

public class when_providing_from_two_providers
{
    static MetadataProviders providers;
    static Metadata.Entry first_provider_first_entry;
    static Metadata.Entry first_provider_second_entry;
    static Metadata.Entry second_provider_first_entry;
    static Metadata.Entry second_provider_second_entry;
    static IEnumerable<Metadata.Entry> result;

    Establish context = () =>
    {
        first_provider_first_entry = new Metadata.Entry("1-1", "Value for 1:1");
        first_provider_second_entry = new Metadata.Entry("1-2", "Value for 1:2");
        second_provider_first_entry = new Metadata.Entry("2-1", "Value for 2:1");
        second_provider_second_entry = new Metadata.Entry("2-2", "Value for 2:2");

        var firstProvider = new Mock<ICanProvideClientMetadata>();
        firstProvider.Setup(_ => _.Provide()).Returns(new[] { first_provider_first_entry, first_provider_second_entry });
        var secondProvider = new Mock<ICanProvideClientMetadata>();
        secondProvider.Setup(_ => _.Provide()).Returns(new[] { second_provider_first_entry, second_provider_second_entry });

        var instances = new StaticInstancesOf<ICanProvideClientMetadata>(firstProvider.Object, secondProvider.Object);
        providers = new MetadataProviders(instances);
    };

    Because of = () => result = providers.Provide();

    It should_have_all_metadata = () => result.ShouldContainOnly(
        first_provider_first_entry,
        first_provider_second_entry,
        second_provider_first_entry,
        second_provider_second_entry);
}