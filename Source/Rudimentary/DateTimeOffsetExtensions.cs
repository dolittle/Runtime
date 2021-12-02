// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Extensions for DateTime.
/// </summary>
public static class DateTimeOffsetExtensions
{
    /// <summary>
    /// Performs a lossy equals algorithm.
    /// </summary>
    /// <param name="leftHandSide"><see cref="DateTimeOffset"/> to check equality for.</param>
    /// <param name="rightHandSide"><see cref="DateTimeOffset"/> to check equality with.</param>
    /// <returns>true if equal, false if not.</returns>
    public static bool LossyEquals(this DateTimeOffset leftHandSide, DateTimeOffset rightHandSide)
    {
        var leftHandSideUtc = leftHandSide.UtcDateTime;
        var rightHandSideUtc = rightHandSide.UtcDateTime;
        return
            leftHandSideUtc.Millisecond == rightHandSideUtc.Millisecond
            && leftHandSideUtc.Second == rightHandSideUtc.Second
            && leftHandSideUtc.Day == rightHandSideUtc.Day
            && leftHandSideUtc.Year == rightHandSideUtc.Year
            && leftHandSideUtc.Ticks >> 10 == leftHandSideUtc.Ticks >> 10;
    }

    /// <summary>
    /// Performs a lossy equals algorithm.
    /// </summary>
    /// <param name="leftHandSide"><see cref="DateTimeOffset"/> to check equality for.</param>
    /// <param name="rightHandSide"><see cref="DateTimeOffset"/> to check equality with.</param>
    /// <returns>true if equal, false if not.</returns>
    public static bool LossyEquals(this DateTimeOffset leftHandSide, DateTime rightHandSide)
        => LossyEquals(leftHandSide, new DateTimeOffset(rightHandSide.ToUniversalTime()));
}