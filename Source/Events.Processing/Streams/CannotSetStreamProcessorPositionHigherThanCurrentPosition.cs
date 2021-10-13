// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.Events.Store.Streams;
namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when attempting to set a stream processor position that is higher than the current . 
    /// </summary>
    public class CannotSetStreamProcessorPositionHigherThanCurrentPosition : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadySettingNewStreamProcessorPosition"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        public CannotSetStreamProcessorPositionHigherThanCurrentPosition(IStreamProcessorId streamProcessorId)
            : base($"Stream Processor: '{streamProcessorId}' is already being set to a new position")
        {
        }
    }
}