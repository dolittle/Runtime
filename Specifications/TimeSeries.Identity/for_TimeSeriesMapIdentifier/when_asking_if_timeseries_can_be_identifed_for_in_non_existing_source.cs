// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Identity.for_TimeSeriesMapIdentifier
{
    public class when_asking_if_timeseries_can_be_identifed_for_in_non_existing_source
    {
        const string source = "MySource";
        const string tag = "MyTag";

        static bool result;

        static TimeSeriesMapIdentifier identifier;

        Establish context = () => identifier = new TimeSeriesMapIdentifier(new TimeSeriesMap(new Dictionary<Source, TimeSeriesByTag>()));

        Because of = () => result = identifier.CanIdentify(source, tag);

        It should_consider_not_having_it = () => result.ShouldBeFalse();
    }
}