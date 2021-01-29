// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using Autofac;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ITenantKeyCreator"/>.
    /// </summary>
    public class TenantKeyCreator : ITenantKeyCreator
    {
        IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantKeyCreator"/> class.
        /// </summary>
        /// <param name="containerBuilder"><see cref="ContainerBuilder"/> used for building the container.</param>
        public TenantKeyCreator(ContainerBuilder containerBuilder)
            => containerBuilder
                .RegisterBuildCallback(c => _executionContextManager = c.Resolve<IExecutionContextManager>());

        /// <inheritdoc/>
        public string GetKeyFor(Binding binding, Type service)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(_executionContextManager.Current.Tenant);
            stringBuilder.Append("-");
            stringBuilder.Append(binding.Service.AssemblyQualifiedName);
            if (service.IsGenericType)
                service.GetGenericArguments().ForEach(_ => stringBuilder.Append($"-{_.AssemblyQualifiedName}"));

            return stringBuilder.ToString();
        }
    }
}