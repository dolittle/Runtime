// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Versioning;
using Machine.Specifications;

namespace Dolittle.Runtime.Platform.Handshake.for_VerifyContractsCompatibility.when_checking_compatibility;

public class and_runtime_patch_is_greater : given.a_verifier_and_versions
{
    Establish context = () =>
    {
        runtime_contracts_version = new Version(7, 4, 7);
        head_contracts_version = new Version(7, 4, 6);
    };

    static ContractsCompatibility result;

    Because of = () => result = verifier.CheckCompatibility(runtime_contracts_version, head_contracts_version);

    It should_return_compatible = () => result.ShouldEqual(ContractsCompatibility.Compatible);
}