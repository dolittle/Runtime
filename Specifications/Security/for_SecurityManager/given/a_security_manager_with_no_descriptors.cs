// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Security.Specs.for_SecurityManager.given
{
    public class a_security_manager_with_no_descriptors
    {
        protected static Mock<IInstancesOf<ISecurityDescriptor>> security_descriptors;
        protected static SecurityManager security_manager;

        Establish context = () =>
            {
                security_descriptors = new Mock<IInstancesOf<ISecurityDescriptor>>();
                security_descriptors.Setup(_ => _.GetEnumerator()).Returns(new List<ISecurityDescriptor>().GetEnumerator());
                security_manager = new SecurityManager(security_descriptors.Object);
            };
    }
}