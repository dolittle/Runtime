// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using grpc = contracts::Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the information that is in the processing response when the processing of an event failed.
    /// </summary>
    public class ProcessorFailure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorFailure"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ProcessorFailureType" />.</param>
        /// <param name="reason">The reason for the failure.</param>
        public ProcessorFailure(ProcessorFailureType type, string reason)
        {
            Type = type;
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorFailure"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ProcessorFailureType" />.</param>
        /// <param name="reason">The reason for the failure.</param>
        /// <param name="retry">Whether it should retry processing.</param>
        /// <param name="retryTimeout">The <see cref="TimeSpan" /> timeout for when to try processing again.</param>
        public ProcessorFailure(ProcessorFailureType type, string reason, bool retry, TimeSpan retryTimeout)
        {
            Type = type;
            Reason = reason;
            Retry = retry;
            RetryTimeout = retryTimeout;
        }

        /// <summary>
        /// Gets the <see cref="ProcessorFailureType" />.
        /// </summary>
        public ProcessorFailureType Type { get; }

        /// <summary>
        /// Gets the reason for failure.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets a value indicating whether to retry processing.
        /// </summary>
        public bool Retry { get; }

        /// <summary>
        /// Gets the retry timeout <see cref="TimeSpan" />.
        /// </summary>
        public TimeSpan RetryTimeout { get; }

        /// <summary>
        /// Implicitly converts the <see cref="grpc.ProcessorFailure" /> to <see cref="ProcessorFailure" />.
        /// </summary>
        /// <param name="failure">The <see cref="grpc.ProcessorFailure" />.</param>
        public static implicit operator ProcessorFailure(grpc.ProcessorFailure failure)
        {
            if (failure == null) return null;
            if (failure.Retry) return new ProcessorFailure((ProcessorFailureType)failure.FailureType, failure.Reason, failure.Retry, failure.RetryTimeout.ToTimeSpan());
            else return new ProcessorFailure((ProcessorFailureType)failure.FailureType, failure.Reason);
        }
    }
}