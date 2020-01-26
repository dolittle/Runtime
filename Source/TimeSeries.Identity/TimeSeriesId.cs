// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents the concept of an System.
    /// </summary>
    public class TimeSeriesId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="TimeSeriesId"/>.
        /// </summary>
        /// <param name="value">TimeSeries as <see cref="Guid"/>.</param>
        public static implicit operator TimeSeriesId(Guid value) => new TimeSeriesId { Value = value };

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="TimeSeriesId"/>.
        /// </summary>
        /// <param name="value">TimeSeries as <see cref="string"/> representation of a <see cref="Guid"/>.</param>
        public static implicit operator TimeSeriesId(string value) => new TimeSeriesId { Value = Guid.Parse(value) };
    }
}