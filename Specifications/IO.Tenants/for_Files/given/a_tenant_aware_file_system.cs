// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.IO.Tenants.for_Files.given
{
    public class a_tenant_aware_file_system
    {
        protected static Microservice microservice = Guid.Parse("d90df5d0-ed59-478a-98ed-967c1bd06364");
        protected static TenantId tenant = Guid.Parse("e00adddf-2fa5-472d-8d27-1314c6dacf0d");
        protected static Versioning.Version version = Versioning.Version.NotSet;
        protected static ExecutionContext execution_context;
        protected static Mock<IFileSystem> file_system;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Files tenant_aware_file_system;
        protected static FilesConfiguration configuration;

        Establish context = () =>
        {
            execution_context = new ExecutionContext(
                microservice,
                tenant,
                version,
                Execution.Environment.Development,
                CorrelationId.New(),
                Claims.Empty,
                CultureInfo.InvariantCulture);
            execution_context_manager = new Mock<IExecutionContextManager>();
            execution_context_manager.SetupGet(_ => _.Current).Returns(execution_context);
            file_system = new Mock<IFileSystem>();
            configuration = new FilesConfiguration { RootPath = "/some_place/with_all_tenants" };
            var configurationFor = new Mock<IConfigurationFor<FilesConfiguration>>();
            configurationFor.SetupGet(_ => _.Instance).Returns(configuration);
            tenant_aware_file_system = new Files(execution_context_manager.Object, configurationFor.Object, file_system.Object);
        };

        protected static string MapPath(string relativePath)
        {
            return Path.Combine(configuration.RootPath, execution_context.Tenant.ToString(), relativePath);
        }
    }
}