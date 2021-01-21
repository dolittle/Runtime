// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;
using Machine.Specifications;

namespace Dolittle.Build.for_ConfigurationObjectProvider
{
    public class when_asking_if_can_provide_for_configuration_object_type_used_by_performer_with_two_registered_performers : given.all_dependencies
    {
        class first_config_object : IConfigurationObject { }

        class first_performer : ICanPerformBuildTask
        {
            public first_performer(first_config_object configObject) { }

            public string Message => string.Empty;

            public void Perform() { }
        }

        class second_config_object : IConfigurationObject { }

        class second_performer : ICanPerformBuildTask
        {
            public second_performer(second_config_object configObject) { }

            public string Message => string.Empty;

            public void Perform() { }
        }

        static ConfigurationObjectProvider provider;
        static bool result;

        Establish context = () =>
        {
            type_finder.Setup(_ => _.FindMultiple<ICanPerformBuildTask>()).Returns(new[]
            {
                typeof(first_performer),
                typeof(second_performer)
            });

            provider = new ConfigurationObjectProvider(type_finder.Object, get_container);
        };

        Because of = () => result = provider.CanProvide(typeof(first_config_object));

        It should_be_able_to_provide = () => result.ShouldBeTrue();
    }
}