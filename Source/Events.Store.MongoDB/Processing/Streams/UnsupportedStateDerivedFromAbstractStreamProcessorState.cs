// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when trying to use an unsuported <see cref="IStreamProcessorState"/> and <see cref="IStreamProcessorId"/> and .
    /// </summary>
    public class UnsupportedStateDerivedFromAbstractStreamProcessorState : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedStateDerivedFromAbstractStreamProcessorState"/> class.
        /// </summary>
        /// <param name="state">A class derived from <see cref="AbstractStreamProcessorState"/>.</param>
        public UnsupportedStateDerivedFromAbstractStreamProcessorState(AbstractStreamProcessorState state)
            : base($"Unsupported StreamProcessorState: {state} derived from AbstractStreamProcessorState")
        {
        }
    }
}
