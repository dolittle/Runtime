// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Heads;
using Dolittle.Services;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> - providing runtime services
    /// for working with <see cref="TimeSeriesId"/> identities.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly TimeSeriesMapIdentifierService _timeSeriesMapIdentifierService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="timeSeriesMapIdentifierService"><see cref="TimeSeriesMapIdentifierService"/> service.</param>
        public RuntimeServices(TimeSeriesMapIdentifierService timeSeriesMapIdentifierService)
        {
            _timeSeriesMapIdentifierService = timeSeriesMapIdentifierService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "TimeSeries";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_timeSeriesMapIdentifierService, contracts::Dolittle.Runtime.TimeSeries.Identity.TimeSeriesMapIdentifier.BindService(_timeSeriesMapIdentifierService), contracts::Dolittle.Runtime.TimeSeries.Identity.TimeSeriesMapIdentifier.Descriptor)
            };
        }
    }
}