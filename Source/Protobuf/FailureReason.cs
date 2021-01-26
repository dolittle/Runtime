// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents the reason for failure.
    /// </summary>
    public record FailureReason(string Value)
    {
        /// <summary>
        /// Implicitly converts <see cref="string" /> to <see cref="FailureReason" />.
        /// </summary>
        /// <param name="failureReason"><see cref="string" /> to convert.</param>
        public static implicit operator FailureReason(string failureReason) => new(failureReason);
        
        /// <summary>
        /// Implicitly converts <see cref="FailureReason" /> to <see cref="string" />.
        /// </summary>
        /// <param name="failureReason"><see cref="FailureReason" /> to convert.</param>
        public static implicit operator string(FailureReason failureReason) => failureReason.Value;
    }
}
