// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Build.for_ConfigurationObjectProvider
{
    public class when_providing_for_a_well_known_configuration_object : given.all_dependencies
    {
        class config_object : IConfigurationObject { }

        class performer : ICanPerformBuildTask
        {
            public performer(config_object configObject) { }

            public string Message => string.Empty;

            public void Perform() { }
        }

        static ConfigurationObjectProvider provider;
        static string performer_name;
        static config_object result;
        static config_object expected_instance;

        static Mock<IPerformerConfigurationManager> configuration_manager;

        Establish context = () =>
        {
            type_finder.Setup(_ => _.FindMultiple<ICanPerformBuildTask>()).Returns(new[]
            {
                typeof(performer)
            });
            var type = typeof(performer);
            performer_name = $"{type.Namespace}.{type.Name}";
            expected_instance = new config_object();

            provider = new ConfigurationObjectProvider(type_finder.Object, get_container);
            configuration_manager = new Mock<IPerformerConfigurationManager>();
            configuration_manager.Setup(_ => _.GetFor(typeof(config_object), performer_name)).Returns(expected_instance);
            container.Setup(_ => _.Get<IPerformerConfigurationManager>()).Returns(configuration_manager.Object);
        };

        Because of = () => result = provider.Provide(typeof(config_object)) as config_object;

        It should_ask_configuration_manager_for_the_config_for_the_plugin = () => configuration_manager.Verify(_ => _.GetFor(typeof(config_object), performer_name), Moq.Times.Once());
        It should_return_expected_instance = () => result.ShouldEqual(expected_instance);
    }
}