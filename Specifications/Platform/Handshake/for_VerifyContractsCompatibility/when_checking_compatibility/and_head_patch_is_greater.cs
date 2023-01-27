// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Platform.Handshake.for_VerifyContractsCompatibility.when_checking_compatibility;

public class and_head_patch_is_greater : given.a_verifier_and_versions
{
    Establish context = () =>
    {
        runtime_contracts_version = new Version(1, 2, 3);
        head_contracts_version = new Version(1, 2, 42);
    };

    static ContractsCompatibility result;

    Because of = () => result = verifier.CheckCompatibility(runtime_contracts_version, head_contracts_version);

    It should_return_compatible = () => result.Should().Be(ContractsCompatibility.Compatible);
}