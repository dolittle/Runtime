// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.IO.Tenants.for_Files
{
    public class when_writing_a_valid_file : given.a_tenant_aware_file_system
    {
        const string path = "some/place/file.txt";
        const string content = "Some result";

        Because of = () => tenant_aware_file_system.WriteAllText(path, content);

        It should_delegate_to_underlying_file_system_and_pass_content_along = () => file_system.Verify(_ => _.WriteAllText(MapPath(path), content), Moq.Times.Once());
    }
}