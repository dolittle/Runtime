// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="IEventProcessorsRegistration" />.
    /// </summary>
    public abstract class AbstractEventProcessorsRegistration : IEventProcessorsRegistration
    {
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        bool _registering;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventProcessorsRegistration"/> class.
        /// </summary>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        protected AbstractEventProcessorsRegistration(
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            StreamProcessorRegistrations = new StreamProcessorRegistrations();
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
        }

        /// <inheritdoc/>
        public bool Completed { get; private set; }

        /// <inheritdoc/>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// Gets the <see cref="CancellationToken" />.
        /// </summary>
        protected CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Gets the <see cref="StreamProcessorRegistrations" />.
        /// </summary>
        protected StreamProcessorRegistrations StreamProcessorRegistrations { get; }

        /// <inheritdoc/>
        public async Task Complete()
        {
            ThrowIfCompleted();
            Completed = true;
            await OnCompleted().ConfigureAwait(false);
            if (Succeeded) StreamProcessorRegistrations.TryStart();
        }

        /// <inheritdoc/>
        public Task<EventProcessorsRegistrationResult> Register()
        {
            ThrowIfCompleted();
            ThrowIfRegistering();
            _registering = true;
            return PerformRegistration();
        }

        /// <inheritdoc/>
        public Task Fail()
        {
            ThrowIfCompleted();
            Succeeded = false;
            return Complete();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Perform the actual registration.
        /// </summary>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="EventProcessorsRegistrationResult" />.</returns>
        protected abstract Task<EventProcessorsRegistrationResult> PerformRegistration();

        /// <summary>
        /// The action that is should be performed only once when the registration is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
        protected virtual Task OnCompleted() => Task.CompletedTask;

        /// <summary>
        /// Register stream processor for all Tenants.
        /// </summary>
        /// <param name="createEventProcessor">A <see cref="Func{TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="IEventProcessor" />.</param>
        /// <param name="getStreamDefinition">A <see cref="Func{TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="StreamDefinition" />. </param>
        /// <typeparam name="TEventProcessor">The <see cref="IEventProcessor" /> type.</typeparam>
        /// <returns>A <see cref="Task" /> that, when resolved, returns whether there are any failing <see cref="StreamProcessorRegistration" /> in <see cref="StreamProcessorRegistrations" />.</returns>
        protected async Task<bool> RegisterStreamProcessor<TEventProcessor>(Func<Task<TEventProcessor>> createEventProcessor, Func<Task<StreamDefinition>> getStreamDefinition)
            where TEventProcessor : IEventProcessor
        {
            await _streamProcessorForAllTenants.Register(createEventProcessor, getStreamDefinition, StreamProcessorRegistrations, CancellationToken).ConfigureAwait(false);
            return StreamProcessorRegistrations.HasFailures;
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                StreamProcessorRegistrations.Dispose();
            }

            Disposed = true;
            if (!Completed) Complete().GetAwaiter().GetResult();
        }

        void ThrowIfRegistering()
        {
            if (_registering) throw new EventProcessorsRegistrationCanOnlyRegisterOnce();
        }

        void ThrowIfCompleted()
        {
            if (Completed) throw new CannotPerformOperationOnCompletedEventProcessorsRegistration();
        }
    }
}