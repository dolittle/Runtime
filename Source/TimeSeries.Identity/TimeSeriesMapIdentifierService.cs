// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Dolittle.TimeSeries.Identity.Runtime.TimeSeriesMapIdentifier;
using grpc = Dolittle.TimeSeries.Identity.Runtime;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents an implementation of <see cref="TimeSeriesMapIdentifierBase"/>.
    /// </summary>
    public class TimeSeriesMapIdentifierService : TimeSeriesMapIdentifierBase
    {
        readonly TimeSeriesMapIdentifier _timeSeriesMapIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesMapIdentifierService"/> class.
        /// </summary>
        /// <param name="timeSeriesMapIdentifier"><see cref="TimeSeriesMapIdentifier"/> for identifying TimeSeries.</param>
        public TimeSeriesMapIdentifierService(TimeSeriesMapIdentifier timeSeriesMapIdentifier)
        {
            _timeSeriesMapIdentifier = timeSeriesMapIdentifier;
        }

        /// <inheritdoc/>
        public override Task<Empty> Register(grpc.TimeSeriesMap request, ServerCallContext context)
        {
            foreach ((var tag, var timeSeriesId) in request.TagToTimeSeriesId)
            {
                _timeSeriesMapIdentifier.Register(request.Source, tag, timeSeriesId.To<TimeSeriesId>());
            }

            return Task.FromResult(new Empty());
        }
    }
}