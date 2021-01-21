// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Machine.Specifications;
using Moq;
using It=Machine.Specifications.It;

namespace Dolittle.Runtime.IO.Tenants.for_Files
{
    public class when_checking_if_file_exists_with_rooted_path_unix : given.a_tenant_aware_file_system
    {
        static Exception result;

        Because of = () => result = Catch.Exception(() => tenant_aware_file_system.Exists("/someplace/somefile.txt"));

        It should_throw_access_outside_sandbox_denied = () => result.ShouldBeOfExactType<AccessOutsideSandboxDenied>();
    }
}