// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Build.for_ConfigurationObjectProvider
{
    public class when_performer_has_more_than_one_constructor : given.all_dependencies
    {
        class performer_with_two_constructors : ICanPerformBuildTask
        {
            public performer_with_two_constructors(string something) { }

            public performer_with_two_constructors(string something, string somethingElse) { }

            public string Message => string.Empty;

            public void Perform()
            {
            }
        }

        static Exception result;

        Establish context = () =>
        {
            type_finder.Setup(_ => _.FindMultiple<ICanPerformBuildTask>()).Returns(new[]
            {
                typeof(performer_with_two_constructors)
            });
        };

        Because of = () => result = Catch.Exception(() => new ConfigurationObjectProvider(type_finder.Object, get_container));

        It should_throw_ambiguous_constructor = () => result.ShouldBeOfExactType<AmbiguousConstructor>();
    }
}