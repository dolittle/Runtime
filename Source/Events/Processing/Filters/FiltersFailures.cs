// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Event Handlers.
/// </summary>
public static class FiltersFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'NoFilterRegistrationReceived' failure type.
    /// </summary>
    public static readonly FailureId NoFilterRegistrationReceived = FailureId.Create("d6060ba0-39bd-4815-8b0e-6b43b5f87bc5");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'ExecutionContextIsInvalid' failure type.
    /// </summary>
    public static readonly FailureId ExecutionContextIsInvalid = FailureId.Create("92b7d18e-0a8a-44b9-bbee-abee206701c5");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'CannotRegisterFilterOnNonWriteableStream' failure type.
    /// </summary>
    public static readonly FailureId CannotRegisterFilterOnNonWriteableStream = FailureId.Create("2cdb6143-4f3d-49cb-bd58-68fd1376dab1");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'FailedToRegisterFilter' failure type.
    /// </summary>
    public static readonly FailureId FailedToRegisterFilter = FailureId.Create("f0480899-8aed-4191-b339-5121f4d9f2e2");
}
