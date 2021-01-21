// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;
using Machine.Specifications;

namespace Dolittle.Build.for_ConfigurationObjectProvider
{
    public class when_asking_if_can_provide_for_configuration_object_type_used_by_performer : given.all_dependencies
    {
        class config_object : IConfigurationObject { }

        class performer : ICanPerformBuildTask
        {
            public performer(config_object configObject) { }

            public string Message => string.Empty;

            public void Perform() { }
        }

        static ConfigurationObjectProvider provider;
        static bool result;

        Establish context = () =>
        {
            type_finder.Setup(_ => _.FindMultiple<ICanPerformBuildTask>()).Returns(new[]
            {
                typeof(performer)
            });

            provider = new ConfigurationObjectProvider(type_finder.Object, get_container);
        };

        Because of = () => result = provider.CanProvide(typeof(config_object));

        It should_be_able_to_provide = () => result.ShouldBeTrue();
    }
}