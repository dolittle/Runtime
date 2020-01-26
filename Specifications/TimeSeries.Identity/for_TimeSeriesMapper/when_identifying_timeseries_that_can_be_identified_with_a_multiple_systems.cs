// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Identity.for_TimeSeriesMapper
{
    public class when_identifying_timeseries_that_can_be_identified_with_a_multiple_systems
    {
        const string source = "Some source";
        const string tag = "Some tag";
        static TimeSeriesMapper mapper;
        static Mock<ICanIdentifyTimeSeries> first_identifier;
        static Mock<ICanIdentifyTimeSeries> second_identifier;
        static Exception result;

        Establish context = () =>
        {
            first_identifier = new Mock<ICanIdentifyTimeSeries>();
            first_identifier.Setup(_ => _.CanIdentify(source, tag)).Returns(true);
            second_identifier = new Mock<ICanIdentifyTimeSeries>();
            second_identifier.Setup(_ => _.CanIdentify(source, tag)).Returns(true);

            mapper = new TimeSeriesMapper(new StaticInstancesOf<ICanIdentifyTimeSeries>(
                first_identifier.Object,
                second_identifier.Object));
        };

        Because of = () => result = Catch.Exception(() => mapper.Identify(source, tag));

        It should_throw_ambiguous_time_series_identifiers = () => result.ShouldBeOfExactType<AmbiguousTimeSeriesIdentifiers>();
    }
}