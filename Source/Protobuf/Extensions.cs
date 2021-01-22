// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Google.Protobuf;
using FailureContract = Dolittle.Protobuf.Contracts.Failure;
using UuidContract = Dolittle.Protobuf.Contracts.Uuid;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents conversion extensions for the common types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert a <see cref="ByteString"/> to a <see cref="ConceptAs{T}"/> of type <see cref="Guid"/>.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="id"><see cref="UuidContract"/> to convert.</param>
        /// <returns>Converted <see cref="ConceptAs{T}"/> of type <see cref="Guid"/>.</returns>
        public static T To<T>(this UuidContract id)
            where T : ConceptAs<Guid>, new() => new T { Value = id.ToGuid() };

        /// <summary>
        /// Convert a <see cref="UuidContract"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id"><see cref="UuidContract"/> to convert.</param>
        /// <returns>Converted <see cref="Guid"/>.</returns>
        public static Guid ToGuid(this UuidContract id) => new Guid(id.Value.ToByteArray());

        /// <summary>
        /// Convert a <see cref="Guid"/> to <see cref="UuidContract"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> to convert.</param>
        /// <returns>Converted <see cref="UuidContract"/>.</returns>
        public static UuidContract ToProtobuf(this Guid id) =>
            new UuidContract { Value = ByteString.CopyFrom(id.ToByteArray()) };

        /// <summary>
        /// Convert a <see cref="ConceptAs{T}"/> of type <see cref="Guid"/> to <see cref="UuidContract"/>.
        /// </summary>
        /// <param name="id"><see cref="ConceptAs{T}"/> of type <see cref="Guid"/> to convert.</param>
        /// <returns>Converted <see cref="UuidContract"/>.</returns>
        public static UuidContract ToProtobuf(this ConceptAs<Guid> id) => id.Value.ToProtobuf();

        /// <summary>
        /// Convert a <see cref="Failure" /> to <see cref="FailureContract" />.
        /// </summary>
        /// <param name="failure"><see cref="Failure" /> to convert.</param>
        /// <returns>Converted <see cref="FailureContract" />.</returns>
        public static FailureContract ToProtobuf(this Failure failure) =>
            new FailureContract { Id = failure.Id.ToProtobuf(), Reason = failure.Reason };

        /// <summary>
        /// Convert a <see cref="FailureContract" /> to <see cref="Failure" />.
        /// </summary>
        /// <param name="failure"><see cref="FailureContract" /> to convert.</param>
        /// <returns>Converted <see cref="Failure" />.</returns>
        public static Failure ToFailure(this FailureContract failure) =>
            new Failure(failure.Id.To<FailureId>(), failure.Reason);
    }
}
