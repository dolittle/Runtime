// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorsRegistration" /> that manages the registration of an Event Processor.
    /// </summary>
    public class EventProcessorRegistration : IEventProcessorsRegistration
    {
        readonly StreamId _sourceStreamId;
        readonly IEventProcessor _eventProcessor;
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        readonly FactoryFor<IStreamDefinitions> _getStreamDefinitions;
        readonly StreamProcessorRegistrations _streamProcessorRegistrations;
        readonly CancellationToken _cancellationToken;

        bool _registering;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistration"/> class.
        /// </summary>
        /// <param name="sourceStreamId">The source <see cref="StreamId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="getStreamDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitions" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public EventProcessorRegistration(
            StreamId sourceStreamId,
            IEventProcessor eventProcessor,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            FactoryFor<IStreamDefinitions> getStreamDefinitions,
            CancellationToken cancellationToken)
        {
            _sourceStreamId = sourceStreamId;
            _eventProcessor = eventProcessor;
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
            _getStreamDefinitions = getStreamDefinitions;
            _streamProcessorRegistrations = new StreamProcessorRegistrations();
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public bool Completed { get; private set; }

        /// <inheritdoc/>
        public bool Succeeded { get; private set; }

        /// <inheritdoc/>
        public Task Complete()
        {
            if (Completed) return Task.CompletedTask;
            Completed = true;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task Fail()
        {
            if (Completed) throw new CannotFailCompletedEventProcessorsRegistration();
            Succeeded = false;
            return Complete();
        }

        /// <inheritdoc/>
        public async Task<EventProcessorsRegistrationResult> Register()
        {
            if (_registering) throw new EventProcessorsRegistrationCanOnlyRegisterOnce();
            if (Completed) throw new CannotRegisterOnCompletedEventProcessorsRegistration();
            _registering = true;
            try
            {
                StreamId targetStream = _eventProcessor.Identifier.Value;

                await _streamProcessorForAllTenants.Register(_eventProcessor, () => _getStreamDefinitions().GetFor(_eventProcessor.Scope, _sourceStreamId, _cancellationToken),  _streamProcessorRegistrations, _cancellationToken).ConfigureAwait(false);
                if (_streamProcessorRegistrations.HasFailures())
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed registering Event Processor: '{_eventProcessor.Identifier}' on Stream: '{_sourceStreamId}");
                }

                return new EventProcessorsRegistrationResult();
            }
            catch (Exception ex)
            {
                Succeeded = false;
                return new EventProcessorsRegistrationResult($"Failed registering Event Processor: '{_eventProcessor.Identifier}' on Stream: '{_sourceStreamId}. {ex.Message}'");
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Completed) Complete().GetAwaiter().GetResult();
            if (_disposed) return;
            if (disposing)
            {
                _streamProcessorRegistrations.Dispose();
            }

            _disposed = true;
        }
    }
}