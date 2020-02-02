// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Identity.for_TimeSeriesMapIdentifier
{
    public class when_registering_for_known_source_with_unknown_tag
    {
        const string source = "Some Source";
        const string other_tag = "MyOtherTag";
        const string tag = "Some Tag";
        static TimeSeriesId other_time_series = Guid.NewGuid();
        static TimeSeriesId time_series_id = Guid.NewGuid();
        static TimeSeriesMapIdentifier identifier;

        Establish context = () =>
       {
           identifier = new TimeSeriesMapIdentifier(new TimeSeriesMap(
               new Dictionary<Source, TimeSeriesByTag>
               {
                    { source, new TimeSeriesByTag(new Dictionary<Tag, TimeSeriesId> { { other_tag, other_time_series } }) },
               }));
       };

        Because of = () => identifier.Register(source, tag, time_series_id);

        It should_be_able_to_identify = () => identifier.CanIdentify(source, tag).ShouldBeTrue();
        It should_return_the_correct_identifier = () => identifier.Identify(source, tag).ShouldEqual(time_series_id);
    }
}