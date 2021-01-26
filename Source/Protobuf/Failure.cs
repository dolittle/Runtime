// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FailureContract = Dolittle.Protobuf.Contracts.Failure;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents a failure.
    /// </summary>
    public record Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Failure"/> class.
        /// </summary>
        /// <param name="id"><see cref="FailureId" />.</param>
        /// <param name="reason"><see cref="FailureReason" />.</param>
        public Failure(FailureId id, FailureReason reason)
        {
            Id = id ?? Failures.Unknown;
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Failure"/> class.
        /// </summary>
        /// <param name="reason"><see cref="FailureReason" />.</param>
        public Failure(FailureReason reason)
        {
            Id = Failures.Unknown;
            Reason = reason;
        }

        /// <summary>
        /// Gets the <see cref="FailureId" />.
        /// </summary>
        public FailureId Id { get; init; }

        /// <summary>
        /// Gets the <see cref="FailureReason" />.
        /// </summary>
        public FailureReason Reason { get; init; }

        /// <summary>
        /// Implicitly convert <see cref="Failure" /> to <see cref="FailureContract" />.
        /// </summary>
        /// <param name="failure"><see cref="Failure" /> to convert.</param>
        public static implicit operator FailureContract(Failure failure) =>
            failure != null ?
                new FailureContract { Id = failure.Id.Value.ToProtobuf(), Reason = failure.Reason }
                : null;

        /// <summary>
        /// Implicitly convert <see cref="FailureContract" /> to <see cref="Failure" />.
        /// </summary>
        /// <param name="failure"><see cref="FailureContract" /> to convert.</param>
        public static implicit operator Failure(FailureContract failure) => failure?.ToFailure();
    }
}