// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Serialization.Json;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor.given
{
    public class a_serializer
    {
        protected static Serializer serializer;
        protected static Mock<IInstancesOf<ICanProvideConverters>> converter_provider_instances;
        protected static List<ICanProvideConverters> converter_providers;

        Establish context = () =>
        {
            converter_providers = new List<ICanProvideConverters>();

            converter_provider_instances = new Mock<IInstancesOf<ICanProvideConverters>>();
            converter_provider_instances.Setup(c => c.GetEnumerator()).Returns(() => converter_providers.GetEnumerator());
            serializer = new Serializer(converter_provider_instances.Object);
        };
    }
}
