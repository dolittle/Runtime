// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents the context of a method call to the Runtime.
/// </summary>
/// <param name="ExecutionContext">The execution context of the request.</param>
/// <param name="HeadId">The identifier for the Head that made the request.</param>
public record CallRequestContext(
    ExecutionContext ExecutionContext,
    Guid HeadId);
