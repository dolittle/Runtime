// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents a failure.
/// </summary>
/// <param name="Id">A unique identifier that identifies the type of failure that occured.</param>
/// <param name="Reason">A description of the failure reason.</param>
public record Failure(
    Guid Id,
    string Reason)
{
    /// <summary>
    /// Converts a <see cref="Dolittle.Protobuf.Contracts.Failure"/> to a <see cref="Failure"/>.
    /// </summary>
    /// <param name="failure">The failure to convert.</param>
    /// <returns>The converted failure.</returns>
    public static implicit operator Failure(Dolittle.Protobuf.Contracts.Failure failure)
        => failure switch
        {
          null => null,
          _ => new(failure.Id.ToGuid(), failure.Reason),
        };
}
