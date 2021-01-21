// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamDefinitions" />.
    /// </summary>
    [Singleton]
    public class StreamDefinitions : IStreamDefinitions
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinitions"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="getStreamDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
        public StreamDefinitions(IPerformActionOnAllTenants onAllTenants, FactoryFor<IStreamDefinitionRepository> getStreamDefinitions)
        {
            _onAllTenants = onAllTenants;
            _getStreamDefinitions = getStreamDefinitions;
        }

        /// <inheritdoc/>
        public Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken) =>
            _onAllTenants.PerformAsync(_ => _getStreamDefinitions().Persist(scope, streamDefinition, cancellationToken));

        /// <inheritdoc/>
        public async Task<Try<IStreamDefinition>> TryGet(ScopeId scope, StreamId streamId, CancellationToken cancellationToken)
        {
            IStreamDefinition result = default;

            await _onAllTenants.PerformAsync(async _ =>
                {
                    var tryGetStreamDefinition = await _getStreamDefinitions().TryGet(scope, streamId, cancellationToken).ConfigureAwait(false);
                    if (tryGetStreamDefinition.Success)
                    {
                        var streamDefinition = tryGetStreamDefinition.Result;
                        if (result == default)
                        {
                            result = streamDefinition;
                        }
                        else if (tryGetStreamDefinition.Result != result)
                        {
                            throw new StreamDefinitionNotTheSameForAllTenants(scope, streamId);
                        }
                    }
                }).ConfigureAwait(false);

            return new Try<IStreamDefinition>(result != default, result);
        }
    }
}
