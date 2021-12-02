// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Protobuf;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see>.
/// </summary>
public static class Failures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'Unknown' failure type.
    /// </summary>
    public static FailureId Unknown => FailureId.Create("90e76603-b6ea-403e-a68e-b9b385b92b16");
}