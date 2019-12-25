// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Commands.Validation.Specs.for_CommandValidators.given
{
    public class all_dependencies
    {
        protected static Mock<IInstancesOf<ICommandValidator>> validators_mock;

        Establish context = () => validators_mock = new Mock<IInstancesOf<ICommandValidator>>();
    }
}
