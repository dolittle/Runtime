// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system which controls the registration of event processors.
    /// </summary>
    public interface IEventProcessorsRegistration : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the registration is completed.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Gets a value indicating whether the registration succeeded or not.
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        /// Begins registration.
        /// </summary>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="EventProcessorsRegistrationResult" />.</returns>
        Task<EventProcessorsRegistrationResult> Register();

        /// <summary>
        /// Starts the registered <see cref="StreamProcessor" />s and completes the registration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Complete();

        /// <summary>
        /// Fail the registration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Fail();
    }
}
