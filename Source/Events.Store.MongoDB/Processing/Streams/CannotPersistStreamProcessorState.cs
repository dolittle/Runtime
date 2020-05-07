// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when we can't succefully persist the given <see cref="IStreamProcessorId"/> and <see cref="IStreamProcessorState"/>.
    /// </summary>
    public class CannotPersistStreamProcessorState : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotPersistStreamProcessorState"/> class.
        /// </summary>
        /// <param name="id">The failed id.</param>
        /// <param name="state">The failed state class.</param>
        public CannotPersistStreamProcessorState(IStreamProcessorId id, IStreamProcessorState state)
            : base($"Failed to persist StreamProcessorState with IStreamProcessorId: {id} and IStreamProcessorState: {state}.")
        {
        }
    }
}
