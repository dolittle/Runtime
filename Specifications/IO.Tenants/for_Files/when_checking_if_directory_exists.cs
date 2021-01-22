// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.IO.Tenants.for_Files
{
    public class when_checking_if_directory_exists : given.a_tenant_aware_file_system
    {
        const string path = "some/place";
        const bool expected_result = true;

        static bool result;

        Establish context = () => file_system.Setup(_ => _.DirectoryExists(MapPath(path))).Returns(expected_result);

        Because of = () => result = tenant_aware_file_system.DirectoryExists(path);

        It should_delegate_to_underlying_file_system_and_return_its_result = () => result.ShouldEqual(expected_result);
    }
}