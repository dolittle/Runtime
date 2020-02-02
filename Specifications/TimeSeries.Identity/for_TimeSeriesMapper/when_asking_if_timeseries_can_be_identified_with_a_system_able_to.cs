// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Identity.for_TimeSeriesMapper
{
    public class when_asking_if_timeseries_can_be_identified_with_a_system_able_to
    {
        const string source = "Some source";
        const string tag = "Some tag";
        static TimeSeriesMapper mapper;
        static Mock<ICanIdentifyTimeSeries> identifier;
        static bool result;

        Establish context = () =>
        {
            identifier = new Mock<ICanIdentifyTimeSeries>();
            identifier.Setup(_ => _.CanIdentify(source, tag)).Returns(true);

            mapper = new TimeSeriesMapper(new StaticInstancesOf<ICanIdentifyTimeSeries>(
                identifier.Object));
        };

        Because of = () => result = mapper.CanIdentify(source, tag);

        It should_be_able_to_identify = () => result.ShouldBeTrue();
    }
}