// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.DependencyInversion;
using Dolittle.Logging;
using Grpc.Core;
using static contracts::Dolittle.Runtime.DependencyInversion.Container;

namespace Dolittle.Runtime.DependencyInversion.Management
{
    /// <summary>
    /// Represents the implementation of the <see cref="ContainerBase"/> service.
    /// </summary>
    public class ContainerService : ContainerBase
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerService"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ContainerService(ILogger<ContainerService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<Bindings> GetBindings(GetBindingsRequest request, ServerCallContext context)
        {
            _logger.Debug("Getting all bindings");

            var bindings = new Bindings();
            bindings.Bindings_.AddRange(PostContainerBootStage.AllBindings);
            return Task.FromResult(bindings);
        }
    }
}