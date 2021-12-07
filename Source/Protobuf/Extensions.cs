// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;
using Google.Protobuf;
using FailureContract = Dolittle.Protobuf.Contracts.Failure;
using UuidContract = Dolittle.Protobuf.Contracts.Uuid;

namespace Dolittle.Runtime.Protobuf;

/// <summary>
/// Represents conversion extensions for the common types.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Convert a <see cref="UuidContract"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="id"><see cref="UuidContract"/> to convert.</param>
    /// <returns>Converted <see cref="Guid"/>.</returns>
    public static Guid ToGuid(this UuidContract id) => new(id.Value.ToByteArray());

    /// <summary>
    /// Convert a <see cref="Guid"/> to <see cref="UuidContract"/>.
    /// </summary>
    /// <param name="id"><see cref="Guid"/> to convert.</param>
    /// <returns>Converted <see cref="UuidContract"/>.</returns>
    public static UuidContract ToProtobuf(this Guid id) =>
        new()
        {
            Value = ByteString.CopyFrom(id.ToByteArray())
        };

    /// <summary>
    /// Convert a <see cref="ConceptAs{T}"/> of type <see cref="Guid"/> to <see cref="Contracts.Uuid"/>.
    /// </summary>
    /// <param name="id"><see cref="ConceptAs{T}"/> of type <see cref="Guid"/> to convert.</param>
    /// <returns>Converted <see cref="Contracts.Uuid"/>.</returns>
    public static UuidContract ToProtobuf(this ConceptAs<Guid> id) => id.Value.ToProtobuf();

    /// <summary>
    /// Convert a <see cref="Failure" /> to <see cref="FailureContract" />.
    /// </summary>
    /// <param name="failure"><see cref="Failure" /> to convert.</param>
    /// <returns>Converted <see cref="FailureContract" />.</returns>
    public static FailureContract ToProtobuf(this Failure failure) =>
        new() { Id = failure.Id.Value.ToProtobuf(), Reason = failure.Reason };
        
    /// <summary>
    /// Convert an <see cref="Exception" /> to <see cref="FailureContract" />.
    /// </summary>
    /// <param name="exception"><see cref="Exception" /> to convert.</param>
    /// <returns>Converted <see cref="FailureContract" />.</returns>
    public static FailureContract ToProtobuf(this Exception exception) =>
        new() { Id = Failures.Unknown.ToProtobuf(), Reason = exception.Message };

    /// <summary>
    /// Convert a <see cref="FailureContract" /> to <see cref="Failure" />.
    /// </summary>
    /// <param name="failure"><see cref="FailureContract" /> to convert.</param>
    /// <returns>Converted <see cref="Failure" />.</returns>
    public static Failure ToFailure(this FailureContract failure) =>
        new(failure.Id.ToGuid(), failure.Reason);
}