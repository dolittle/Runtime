// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Serialization.Json;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer.given
{
    public class a_serializer
    {
        protected static Serializer serializer;
        protected static Mock<ILogger> mock_logger;
        protected static Mock<IInstancesOf<ICanProvideConverters>> converter_provider_instances;
        protected static List<ICanProvideConverters> converter_providers;

        Establish context = () =>
                                {
                                    converter_providers = new List<ICanProvideConverters>();

                                    mock_logger = new Mock<ILogger>();

                                    var provider = new Mock<ICanProvideConverters>();
                                    provider.Setup(p => p.Provide()).Returns(new JsonConverter[] { new ConceptConverter(), new ConceptDictionaryConverter(mock_logger.Object) });
                                    converter_providers.Add(provider.Object);
                                    converter_provider_instances = new Mock<IInstancesOf<ICanProvideConverters>>();
                                    converter_provider_instances.Setup(c => c.GetEnumerator()).Returns(() => converter_providers.GetEnumerator());
                                    serializer = new Serializer(converter_provider_instances.Object);
                                };
    }
}
