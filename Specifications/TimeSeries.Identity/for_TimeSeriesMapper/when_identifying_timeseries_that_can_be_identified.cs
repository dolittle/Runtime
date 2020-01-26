// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Identity.for_TimeSeriesMapper
{
    public class when_identifying_timeseries_that_can_be_identified
    {
        const string source = "Some source";
        const string tag = "Some tag";
        static TimeSeriesId time_series_id = Guid.NewGuid();
        static TimeSeriesMapper mapper;
        static Mock<ICanIdentifyTimeSeries> identifier;
        static TimeSeriesId result;

        Establish context = () =>
        {
            identifier = new Mock<ICanIdentifyTimeSeries>();
            identifier.Setup(_ => _.CanIdentify(source, tag)).Returns(true);
            identifier.Setup(_ => _.Identify(source, tag)).Returns(time_series_id);

            mapper = new TimeSeriesMapper(new StaticInstancesOf<ICanIdentifyTimeSeries>(
                identifier.Object));
        };

        Because of = () => result = mapper.Identify(source, tag);

        It should_return_time_series_id = () => result.ShouldEqual(time_series_id);
    }
}