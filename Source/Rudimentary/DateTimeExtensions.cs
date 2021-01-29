// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary
{
    /// <summary>
    /// Extensions for DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Performs a lossy equals algorithm.
        /// </summary>
        /// <param name="leftHandSide"><see cref="DateTime"/> to check equality for.</param>
        /// <param name="rightHandSide"><see cref="DateTimeOffset"/> to check equality with.</param>
        /// <returns>true if equal, false if not.</returns>
        public static bool LossyEquals(this DateTime leftHandSide, DateTimeOffset rightHandSide)
            => rightHandSide.LossyEquals(leftHandSide);

        /// <summary>
        /// Performs a lossy equals algorithm.
        /// </summary>
        /// <param name="leftHandSide"><see cref="DateTime"/> to check equality for.</param>
        /// <param name="rightHandSide"><see cref="DateTime"/> to check equality with.</param>
        /// <returns>true if equal, false if not.</returns>
        public static bool LossyEquals(this DateTime leftHandSide, DateTime rightHandSide)
            => new DateTimeOffset(leftHandSide.ToUniversalTime()).LossyEquals(rightHandSide.ToUniversalTime());

        /// <summary>
        /// Converts the date to Unix Time in Milliseconds.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> to convert.</param>
        /// <returns>Time in uniq time milliseconds (Epoch).</returns>
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
            => new DateTimeOffset(dateTime.ToUniversalTime(), TimeSpan.Zero)
                .ToUniversalTime()
                .ToUnixTimeMilliseconds();

        /// <summary>
        /// Converts the unix milliseconds to a DateTime.
        /// </summary>
        /// <param name="milliseconds">The epoch time in milliseconds.</param>
        /// <returns>Converted <see cref="DateTime"/>.</returns>
        public static DateTime ToDateTime(this long milliseconds)
            => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).ToUniversalTime().UtcDateTime;

        /// <summary>
        /// Converts the unix milliseconds to a DateTime.
        /// </summary>
        /// <param name="milliseconds">The epoch time in milliseconds.</param>
        /// <returns>Converted <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset ToDateTimeOffset(this long milliseconds)
            => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).ToUniversalTime();
    }
}