// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Booting;
using Dolittle.Runtime.DependencyInversion.Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IContainer = Dolittle.Runtime.DependencyInversion.IContainer;

namespace Dolittle.Runtime.Hosting.Microsoft
{
    /// <summary>
    /// Represents an implementation of <see cref="IServiceProviderFactory{T}"/> that builds upon the <see cref="AutofacServiceProviderFactory"/> and adds services from the Dolittle boot process.
    /// </summary>
    public class ServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        readonly AutofacServiceProviderFactory _autofacFactory;
        readonly HostBuilderContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="context">The <see cref="HostBuilderContext"/>.</param>
        public ServiceProviderFactory(HostBuilderContext context)
        {
            _autofacFactory = new AutofacServiceProviderFactory();
            _context = context;
        }

        /// <inheritdoc/>
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            ApplyLoggerFactoryWorkarounds(services);

            var builder = _autofacFactory.CreateBuilder(services);

            var bootResult = Bootloader.Configure(_ =>
            {
                if (_context.HostingEnvironment.IsDevelopment()) _.Development();
                _.SkipBootprocedures();
                _.UseContainer<ServiceProviderContainer>();
            }).Start();

            builder.AddDolittle(bootResult.Assemblies, bootResult.Bindings);

            return builder;
        }

        /// <inheritdoc/>
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var serviceProvider = _autofacFactory.CreateServiceProvider(containerBuilder);
            try
            {
                var container = serviceProvider.GetService(typeof(IContainer)) as IContainer;
                DependencyInversion.Booting.Boot.ContainerReady(container);
                BootStages.ContainerReady(container);

                var bootProcedures = container.Get<IBootProcedures>();
                bootProcedures.Perform();
            }
            finally
            {
            }

            return serviceProvider;
        }

        void ApplyLoggerFactoryWorkarounds(IServiceCollection services)
        {
            /* Microsoft seems to always bind the ILoggerFactory to their own implementation of LoggerFactory. There is two problems with this:
             * 1) Autofac is not able to pick the best constructor of that type.
             * 2) If any other ILoggerFactory implementation has been bound, it is probably because the developer has added a third party logging framework, which will be ignored.
             */

            var defaultLoggerFactory = services.FirstOrDefault(_ => _.ServiceType == typeof(ILoggerFactory) && _.ImplementationType == typeof(LoggerFactory));
            if (defaultLoggerFactory != null)
            {
                // So we remove the default binding.
                services.Remove(defaultLoggerFactory);

                // And if there are no other bindings - meaning developer bound implementations.
                if (!services.Any(_ => _.ServiceType == typeof(ILoggerFactory)))
                {
                    // We bind the ILoggerFactory explicitly to the correct constructor.
                    // See: https://github.com/aspnet/Logging/issues/691
                    services.AddSingleton<ILoggerFactory>(serviceProvider =>
                        new LoggerFactory(
                            serviceProvider.GetRequiredService<IEnumerable<ILoggerProvider>>(),
                            serviceProvider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>()));
                }
            }
        }
    }
}
