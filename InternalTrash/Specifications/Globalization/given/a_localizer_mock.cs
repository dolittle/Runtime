// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Globalization;
using Machine.Specifications;
using Moq;

namespace Dolittle.Specs.Globalization.given
{
    public class a_localizer_mock
    {
        protected static Mock<ILocalizer> localizer_mock;

        Establish context = () =>
                                {
                                    localizer_mock = new Mock<ILocalizer>();
                                    localizer_mock.Setup(l => l.BeginScope()).Returns(LocalizationScope.FromCurrentThread);
                                };
    }
}
