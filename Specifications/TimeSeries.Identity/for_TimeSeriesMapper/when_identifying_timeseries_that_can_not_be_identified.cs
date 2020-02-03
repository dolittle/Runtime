// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Identity.for_TimeSeriesMapper
{
    public class when_identifying_timeseries_that_can_not_be_identified
    {
        const string source = "Some source";
        const string tag = "Some tag";
        static TimeSeriesMapper mapper;
        static Mock<ICanIdentifyTimeSeries> identifier;
        static Exception result;

        Establish context = () =>
        {
            identifier = new Mock<ICanIdentifyTimeSeries>();
            identifier.Setup(_ => _.CanIdentify(source, tag)).Returns(false);

            mapper = new TimeSeriesMapper(new StaticInstancesOf<ICanIdentifyTimeSeries>(
                identifier.Object));
        };

        Because of = () => result = Catch.Exception(() => mapper.Identify(source, tag));

        It should_throw_no_time_series_identifier_can_identify = () => result.ShouldBeOfExactType<NoTimeSeriesIdentifierCanIdentify>();
    }
}