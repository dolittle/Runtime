// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using Machine.Specifications;

namespace Dolittle.Runtime.Platform.Handshake.for_VerifyContractsCompatibility.given;

public class a_verifier_and_versions
{
    protected static VerifyContractsCompatibility verifier;

    protected static Version runtime_contracts_version;
    protected static Version head_contracts_version;

    Establish context = () =>
    {
        verifier = new VerifyContractsCompatibility();
    };
}